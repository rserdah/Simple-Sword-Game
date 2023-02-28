using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    [Serializable]
    public class Stats
    {
        public float maxHealth = 100f;
        public float stamina = 5f;
        /// <summary>
        /// Player's weapon's damage will be multiplied by this.
        /// </summary>
        public float strength = 1f;
        /// <summary>
        /// Incoming damage will be divided by this.
        /// </summary>
        public float toughness = 1f;

        public float health = 100f;
        public int maxJumps = 2;
        public int jumps = 2;
        /// <summary>
        /// The minimum amount of time before Player can get damaged by the same Enemy
        /// </summary>
        [Tooltip("The minimum amount of time before Player can get damaged by the same Enemy.")]
        public float getHitRestTime = 0.5f;

        public float aimSpeedY = 1f;
        public float aimSpeedX = 50f;

        public Vector3 velocity;

        //Bools
        public bool isGrounded;
        public bool isLockedOn;
        public bool isAiming;
        public bool isBlocking;
        public bool isAttacking;
        /// <summary>
        /// Is true while a Weapon is colliding with character (based on bodyHalfExtents)
        /// </summary>
        public bool isGettingHit;
        public bool isDead;
        public bool isPaused;
        public bool isInAttackingRange;

        //Extents for checking collisions (half extents, not full extents)
        public Vector3 groundCheckHalfExtents = new Vector3(0.5f, 0.5f, 0.5f); //For checking if grounded
        public Vector3 bodyHalfExtents = new Vector3(0.75f, 1.75f, 0.75f); //For checking if getting hit by an Enemy
        public Vector3 selectHalfExtents = new Vector3(1f, 1f, 5f);

        //For Enemies
        public float detectionRadius = 10f;
        public float attackRadius = 5f;
        /// <summary>
        /// The closest this Entity (only used for Enemy) will allow another Entity (usually Player) to get, if distance between the Entity and this Entity is less than minRadius, this Entity will move back in an attempt to increase distance
        /// between itself and the other Entity (so a Player cannot get so close to an Enemy that weapon collisions do not register)
        /// </summary>
        public float minRadius = 2.5f;
    }

    public class Components
    {
        //public Rigidbody rb;
        public CharacterController characterController;
        public Animator anim;
        public NavMeshAgent agent;
        public Collider collider;


        public virtual void GetComponents(Entity entity)
        {
            //rb = enemy.GetComponent<Rigidbody>();
            characterController = entity.GetComponent<CharacterController>();

            if(entity.rig)
                anim = entity.rig.GetComponent<Animator>();

            agent = entity.GetComponent<NavMeshAgent>();
            collider = entity.GetComponent<Collider>();
        }
    }

    public class Collisions
    {
        private Entity entity;


        public void Initialize(Entity e)
        {
            entity = e;
        }

        //Check all GroundCheck methods
        public bool GroundCheck(Vector3 position, Vector3 halfExtents, LayerMask layerMask)
        {
            return Physics.CheckBox(position, halfExtents, entity.quaternionZero, layerMask);
        }

        public bool GroundCheck(Vector3 position, Vector3 halfExtents, string tag)
        {
            return Physics.OverlapBox(position, halfExtents, entity.quaternionZero, entity.layerMasks.everything)[0].tag.Equals(tag);
        }

        public bool GroundCheck(LayerMask layerMask)
        {
            //Physics.OverlapBox() (which is called in this overloaded GroundCheck()) takes half extents, so have to pass in less than 1/2 (hence the * 0.3f) of the CharacterController radius or else it will collide with things on the sides of 
            //the character instead of just under the character
            return GroundCheck(entity.groundCheckOrigin.position, entity.stats.groundCheckHalfExtents, layerMask);
        }

        public bool GroundCheck(string tag)
        {
            //Physics.OverlapBox() (which is called in this overloaded GroundCheck()) takes half extents, so have to pass in less than 1/2 (hence the * 0.3f) of the CharacterController radius or else it will collide with things on the sides of 
            //the character instead of just under the character
            return GroundCheck(entity.groundCheckOrigin.position, entity.stats.groundCheckHalfExtents, tag);
        }

        public bool GroundCheck()
        {
            return GroundCheck(entity.layerMasks.ground);
        }

        public Collider[] GetCollisions(Vector3 position, Vector3 halfExtents, LayerMask layerMask, Quaternion rotation = new Quaternion())
        {
            return Physics.OverlapBox(position, halfExtents, rotation, layerMask);
        }

        public Collider GetCollision(Vector3 position, Vector3 halfExtents, LayerMask layerMask)
        {
            Collider col = null;

            try { col = GetCollisions(position, halfExtents, layerMask)[0]; }
            catch(Exception) { }


            return col;
        }

        public Collider GetGroundCheck(LayerMask layerMask)
        {
            //Physics.OverlapBox() (which is called in this overloaded GetGroundCheck()) takes half extents, so have to pass in less than 1/2 (hence the * 0.3f) of the CharacterController radius or else it will collide with things on the sides of 
            //the character instead of just under the character
            return GetCollision(entity.groundCheckOrigin.position, entity.stats.groundCheckHalfExtents, layerMask);
        }

        public Collider GetGroundCheck(string tag)
        {
            //Physics.OverlapBox() (which is called in this overloaded GetGroundCheck()) takes half extents, so have to pass in less than 1/2 (hence the * 0.3f) of the CharacterController radius or else it will collide with things on the sides of 
            //the character instead of just under the character
            //      return GetGroundCheck(groundCheckOrigin.position, groundCheckHalfExtents, tag);
            return GetCollision(entity.groundCheckOrigin.position, entity.stats.groundCheckHalfExtents, tag);
        }

        public Collider GetCollision(Vector3 position, Vector3 halfExtents, string tag, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            Collider col = null;

            try { col = Physics.OverlapBox(position, halfExtents, entity.quaternionZero, entity.layerMasks.everything, queryTriggerInteraction)[0]; }
            catch(Exception) { }


            if(col && col.tag.Equals(tag))
                return col;
            else
                return null;
        }

        public Collider GetCollisionByTag(Vector3 position, Vector3 halfExtents, string tag, Quaternion rotation = new Quaternion(), QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            Collider col = null;

            try { col = Physics.OverlapBox(position, halfExtents, rotation, entity.layerMasks.everything, queryTriggerInteraction)[0]; }
            catch(Exception) { }


            if(col && col.tag.Equals(tag))
                return col;
            else
                return null;
        }

        public Collider GetWeaponCollision()
        {
            Collider[] weaponCols = GetCollisions(entity.transform.position, entity.stats.bodyHalfExtents, entity.layerMasks.weapon);

            for(int i = 0; i < weaponCols.Length; i++)
                if(!weaponCols[i].transform.Equals(entity.weapon.transform) && !weaponCols[i].transform.Equals(entity.shield.transform))
                    return weaponCols[i];


            return null;
        }

        public Collider GetWeaponCollision(Vector3 position, Quaternion rotation, Vector3 halfExtents)
        {
            Collider[] weaponCols = GetCollisions(position, entity.stats.bodyHalfExtents, entity.layerMasks.weapon, rotation);

            for(int i = 0; i < weaponCols.Length; i++)
                if(!weaponCols[i].transform.Equals(entity.weapon.transform) && !weaponCols[i].transform.Equals(entity.shield.transform))
                    return weaponCols[i];


            return null;
        }
    }

    public enum HealthOrbState
    {
        OPEN, CLOSE, POPUP
    }

    public Stats stats = new Stats();
    public Components components = new Components();
    public Collisions collisions = new Collisions();

    //Main References

    public Transform rig; //First child of all Entities (currently; Eventually may make models in 3D software so this may change); rig has Animator
    public (Transform outer, Material outerMat, Transform inner, Material innerMat, Animator anim, HealthOrbState state, HealthOrbState lastState) healthOrb;
    public (Canvas canvas, Image health, Image stamina, float minBarFill, float maxBarFill) hud;

    public Item weapon;
    public Item shield;
    public Vector3 shieldHalfExtents;

    public Entity duelOpponent;

    public (LayerMask everything, LayerMask ground, LayerMask player, LayerMask enemy, LayerMask weapon, LayerMask shield) layerMasks;

    //public LayerMask playerLayer;
    //public LayerMask enemyLayer;
    //public LayerMask weaponLayer;
    //public LayerMask shieldLayer;


    //Temp./Holder Variables
    protected IEnumerator stopAttackingCoroutine;
    /*protected*/ public float attackDuration = 0.9f; //Temp.; get references to lengths of each attack animation (don't want to use StateMachineBehaviours)
    float nextTimeHit;
    protected Quaternion quaternionZero = new Quaternion();
    public Transform groundCheckOrigin;
    protected Collider col;
    float lastDamageAmount;


    public virtual void Start()
    {
        rig = transform.GetChild(0); //Before getting Components b/c Animator is on rig

        components.GetComponents(this);
        collisions.Initialize(this);

        if(rig)
            healthOrb.outer = rig.Find("HealthOrb"); //Now, HealthOrb is a child of rig (which is 0th child of Entity, but using Find() so this doesn't matter)

        if(healthOrb.outer)
        {
            healthOrb.inner = healthOrb.outer.GetChild(0);
            healthOrb.outerMat = healthOrb.outer.GetComponent<Renderer>().material;
            healthOrb.innerMat = healthOrb.inner.GetComponent<Renderer>().material;
            healthOrb.anim = healthOrb.outer.GetComponent<Animator>();

            if(!(this is TPSPlayer))
                ShowHealthOrb(HealthOrbState.CLOSE);
        }

        try { hud.canvas = transform.Find("HUD").GetComponent<Canvas>(); } catch(Exception) { }

        if(hud.canvas)
        {
            hud.health = hud.canvas.transform.GetChild(0).GetComponent<Image>(); //Health bar is first child of HUD Canvas
            hud.stamina = hud.canvas.transform.GetChild(1).GetComponent<Image>(); //Stamina bar is second child of HUD Canvas

            //Min & Max bar fill are for a circular progress bar that is meant to only fill the top right quarter of the circle (only used for Player)
            hud.minBarFill = 0.75f;
            hud.maxBarFill = 1f;
        }

        layerMasks.everything = ~(0 << 32);
        layerMasks.ground = 1 << LayerMask.NameToLayer("Obstacle");
        layerMasks.player = 1 << LayerMask.NameToLayer("Player");
        layerMasks.enemy = 1 << LayerMask.NameToLayer("Enemy");
        layerMasks.weapon = 1 << LayerMask.NameToLayer("Weapon");
        layerMasks.shield = 1 << LayerMask.NameToLayer("Shield");

        if(weapon)
            weapon.entity = this;
        if(shield)
            shield.entity = this;

        UpdateHealth();
    }

    public virtual void Update()
    {
        if(!stats.isDead)
        {
            if(col = collisions.GetWeaponCollision())
            {
                if(!stats.isGettingHit)
                {
                    Entity attackingEntity = null;
                    Item weapon = col.GetComponent<Item>();
                    if(weapon)
                        attackingEntity = weapon.GetEntity();

                    if(attackingEntity && attackingEntity.stats.isAttacking)
                    {
                        TakeDamage(attackingEntity, CalculateDamage(attackingEntity, weapon));

                        if(!stats.isDead) //Entity can call Die() in TakeDamage() if their health becomes <= 0 in that frame when TakeDamage() is called (so don't play knock back animation after Entity calls Die())
                        {
                            components.anim.Play("GetHit");
                            Debug.LogError(name + " is getting hit");
                        }

                        stats.isGettingHit = true;
                    }
                }
            }
            else
            {
                stats.isGettingHit = false;
            }
        }
    }

    public Vector3 LookAt(Vector3 position, bool zeroLocalXRotation = true)
    {
        Vector3 heading = position - transform.position;

        if(zeroLocalXRotation)
            heading.y = 0;

        heading.Normalize();

        transform.forward = heading;


        return heading;
    }

    public void OnPreDuel(Entity opponent) //Eventually return bool if this Entity can enter a Duel with opponent
    {
        if(!duelOpponent && (!opponent.duelOpponent || opponent.duelOpponent.Equals(this))) //If this Entity is not already in a duel AND (opponent Entity are not already in a duel OR opponent Entity is already in duel with this Entity)
        {
            Debug.LogError($"Entering duel: {name} vs. {opponent.name}");

            duelOpponent = opponent;

            duelOpponent.OnPreDuel(this);
        }
        else
        {
            Debug.LogError($"Cannot enter duel"); //{opponent.name} is already in duel with {opponent.duelOpponent.name}");
        }
    }

    public void OnPostDuel(Entity opponent) //maybe just have no parameter, will just use current duelOpponent
    {
        if(duelOpponent && duelOpponent.Equals(opponent))
        {
            Debug.LogError($"Exitting duel: {name} vs. {opponent.name}");

            Entity e = duelOpponent; //or just use opponent var. b/c duelOpponent and opponent are the same
            duelOpponent = null; //Need to set duelOpponent to null BEFORE OnPostDuel() to avoid infinite loop

            e.OnPostDuel(this);
        }
        else
        {
            Debug.LogError($"Cannot exit duel; was not in a duel {(duelOpponent ? "with " + opponent.name : "")}");
        }
    }

    public virtual Vector3 GetDuelOpponentMoveDirection()
    {
        return Vector3.zero;
    }

    private void OnPreAttack()
    {
        if(stopAttackingCoroutine != null)
            StopCoroutine(stopAttackingCoroutine);

        stopAttackingCoroutine = StopAttacking(attackDuration);

        StartCoroutine(stopAttackingCoroutine);
    }

    public virtual void Attack()
    {
        OnPreAttack();
    }

    /// <summary>
    /// Called after current attack animation ends
    /// </summary>
    public virtual void OnPostAttack()
    {

    }

    public virtual void Block(bool block)
    {
        if(block)
        {
            stats.isBlocking = true;

            components.anim.SetTrigger("SecondaryTrigger");
            components.anim.SetBool("SecondaryIdle", true);
            components.anim.SetFloat("Secondary", 1f);
        }
        else
        {
            stats.isBlocking = false;

            components.anim.SetBool("SecondaryIdle", false);
            components.anim.SetFloat("Secondary", 0f);
        }
    }

    public float CalculateDamage(Entity attackingEntity, Item attackingItem)
    {
        //temp. going to make it so don't need to cast
        if(attackingEntity is TPSPlayer)
            return (attackingItem.GetStrength() * ((TPSPlayer)attackingEntity).stats.strength) / (stats.toughness * (stats.isBlocking ? shield.stats.toughness : 1f));
        else if(attackingEntity is Enemy)
            return (attackingItem.GetStrength() * ((Enemy)attackingEntity).stats.strength) / (stats.toughness * (stats.isBlocking ? shield.stats.toughness : 1f));
        else
            return 0f; //Temp.
    }

    /// <summary>
    /// Optional parameter in order to calculate how much damage to take when blocking
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public bool TakeDamage(Entity attackingEntity, float damage = 0f)
    {
        nextTimeHit = Time.time + stats.getHitRestTime;

        if(stats.health > 0)
        {
            stats.health -= damage;
        }

        lastDamageAmount = damage;
        UpdateHealth();

        if(stats.health <= 0)
        {
            if(attackingEntity)
                attackingEntity.OnKilledEntity(this);

            Die();
        }

        return true;
    }

    public void ShowHealthOrb(HealthOrbState state)
    {
        if(healthOrb.anim && healthOrb.state != state) //If HealthOrb has its Animator and HealthOrb is not already in the state that it it requested to enter
        {
            if(state == HealthOrbState.OPEN)
                healthOrb.anim.Play("Open");
            else if(state == HealthOrbState.CLOSE)
                healthOrb.anim.Play("Close");
            else if(state == HealthOrbState.POPUP)
                healthOrb.anim.Play("Popup");

            healthOrb.lastState = healthOrb.state;
            healthOrb.state = state;
        }
    }

    public void UpdateHealth()
    {
        float healthPercent = Mathf.Clamp01(stats.health / stats.maxHealth);

        if(healthOrb.outer)
        {
            Color healthColor = Color.Lerp(Color.red, Color.green, healthPercent);

            //Option 1
            //foreach(Material m in healthMaterials)
            //    m.SetColor("_EmissionColor", healthColor); //Make selection color based on selected target's health

            //Option 2
            healthOrb.outerMat.SetColor("_EmissionColor", healthColor);
            healthOrb.innerMat.SetColor("_Color", healthColor);
            healthOrb.inner.localScale = Mathf.Clamp01(stats.health / stats.maxHealth) * Vector3.one;

            //Or have both scale (so constant out ring) OR have inside scale faster than outside so has cool effect
        }

        if(hud.health)
        {
            FillBar(hud.health, healthPercent, hud.minBarFill, hud.maxBarFill);
        }

        if(this is TPSPlayer) //temp
        {
            TPSPlayer self = ((TPSPlayer)this);
            self.r.anchoredPosition = self.camera.WorldToScreenPoint(transform.position);
            self.r.GetChild(0).GetComponent<Text>().text = "-" + lastDamageAmount;
        }
    }

    public static void FillBar(Image bar, float fillAmount, float minFill = 0f, float maxFill = 1f)
    {
        if(bar)
            bar.fillAmount = minFill + (fillAmount * (maxFill - minFill));
    }

    private IEnumerator StopAttacking(float delay)
    {
        yield return new WaitForSeconds(delay);

        OnPostAttack();
        stopAttackingCoroutine = null;
    }

    public virtual void OnKilledEntity(Entity entity)
    {

    }

    private void Die()
    {
        stats.isDead = true;

        components.anim.StopPlayback(); //Needed?
        components.anim.SetBool("IsWalking", false);
        components.anim.Play("Die");
        ShowHealthOrb(HealthOrbState.CLOSE);
        components.collider.enabled = false;


        //enabled = false;
    }
}
