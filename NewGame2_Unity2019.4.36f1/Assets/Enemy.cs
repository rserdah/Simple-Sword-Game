using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    [Serializable]
    public class Settings
    {
        public float walkSpeed = 20f;
        public float runSpeed = 35f;

        public float mouseSpeed = 1f;

        public float gravity = -9.81f;
        public float jumpHeight = 2f;
        public bool airControl = true;
        public bool disableAirControlUntilLand;

        public float velocityCheckTimeDelta = 0.5f;

        public float turnSmoothTime = 0.1f;
    }

    //public class Components
    //{
    //    //public Rigidbody rb;
    //    public CharacterController characterController;
    //    public Animator anim;
    //    public Collider collider;


    //    public void GetComponents(Enemy enemy)
    //    {
    //        //rb = enemy.GetComponent<Rigidbody>();
    //        characterController = enemy.GetComponent<CharacterController>();
    //        anim = enemy.GetComponent<Animator>();
    //        collider = enemy.GetComponent<Collider>();
    //    }
    //}

    //[Serializable]
    //public class Stats
    //{
    //    public float maxHealth = 100f;
    //    /// <summary>
    //    /// Player's weapon's damage will be multiplied by this.
    //    /// </summary>
    //    public float strength = 1f;
    //    /// <summary>
    //    /// Incoming damage will be divided by this.
    //    /// </summary>
    //    public float toughness = 1f;

    //    public float health = 100f;
    //    public int maxJumps = 2;
    //    public int jumps = 2;
    //    /// <summary>
    //    /// The minimum amount of time before Player can get damaged by the same Enemy
    //    /// </summary>
    //    [Tooltip("The minimum amount of time before Player can get damaged by the same Enemy.")]
    //    public float getHitRestTime = 0.5f;

    //    public Vector3 velocity;

    //    //Bools
    //    public bool isGrounded;
    //    public bool isBlocking;
    //    /// <summary>
    //    /// Is true while a Weapon is colliding with character (based on bodyHalfExtents)
    //    /// </summary>
    //    public bool isGettingHit;
    //    public bool isDead;

    //    //Extents for checking collisions (half extents, not full extents)
    //    public Vector3 groundCheckHalfExtents = new Vector3(0.5f, 0.5f, 0.5f); //For checking if grounded
    //    public Vector3 bodyHalfExtents = new Vector3(0.75f, 1.75f, 0.75f); //For checking if getting hit by an Enemy
    //}

    //public new class Components : Entity.Components
    //{
    //    public NavMeshAgent agent;


    //    public override void GetComponents(Entity entity)
    //    {
    //        base.GetComponents(entity);


    //        agent = entity.GetComponent<NavMeshAgent>();
    //    }
    //}

    //[Serializable]
    //public new class Stats : Entity.Stats
    //{
    //    //Add new variables if needed

    //    public float detectionRadius = 10f;
    //    public float attackRadius = 5f;
    //}


    public Settings settings = new Settings();
    //public Components components = new Components();
    //public new Components components = new Components();
    //public Stats stats = new Stats();
    //public new Stats stats = new Stats();

    //public Transform groundCheckOrigin;

    //public Transform healthOrbT;
    //(Transform outer, Material outerMat, Transform inner, Material innerMat) healthOrb;

    //public Item weapon;
    //public Item shield;
    //public Vector3 shieldHalfExtents;

    //Collider col = null;
    //Quaternion quaternionZero = new Quaternion();

    [Serializable]
    public struct ActionDelay
    {
        float nextOccurance;
        public bool paused;
        public float min;
        public float max;
        //public bool waiting { get; private set; } //Auto-property, simply a public get property, but can only be set privately (without the need for a separate private var.)
        bool m_waiting;

        public float random
        {
            get
            {
                return UnityEngine.Random.Range(min, max);
            }
        }

        public bool waiting
        {
            get
            {
                if(paused)
                    return false;


                bool isReady = ready;

                m_waiting = !isReady;


                return m_waiting;
            }

            private set
            {
                m_waiting = value;
            }
        }

        public bool ready
        {
            get
            {
                if(paused)
                    return false;


                bool isReady = Time.time - nextOccurance >= 0;

                if(isReady)
                {
                    Debug.LogError($"({Time.time} - {nextOccurance}) {Time.time - nextOccurance} >= 0");
                    waiting = false;
                }


                return isReady;
            }
        }

        public void SetNextRandomOccurance(bool overrideCurrent = false)
        {
            if(!paused)
            {
                if(overrideCurrent || !waiting)
                {
                    nextOccurance = Time.time + random;


                    waiting = true;
                }
            }
        }
    }

    public static int EverythingMask = ~(0 << 32);
    //public LayerMask weaponLayer;
    //public LayerMask playerLayer;

    //float nextTimeHit; //Next time can get hit (not next time going to attack)

    public float time; //temp.
    float nextTimeAttack;
    public float minAttackDelay = 0.5f;
    public float maxAttackDelay = 5f;
    //float nextTimeBlock;
    //public float minBlockDelay = 0.5f;
    //public float maxBlockDelay = 5f;
    public ActionDelay blockDelay;


    public Vector3 randDir;


    /*
     * Maybe make a class called AI or NPC or NPCStats to store info for Enemy NPCs
     * 
        Randomized Actions for Enemies:
         - Choose a random direction to move in while Player is in a duel with this Enemy (when Player is locked on to this Enemy; need callback OnLockedOn() to store this info) & move in this direction for a random amount of time
         - Block with shield in random intervals at random times (when not attacking)
         - Randomly block a Player attack before it lands (needs call back such as OnIncomingAttack() to know this)
     */

    public override void Start()
    {
        base.Start();


        //components.GetComponents(this);

        components.agent.speed = settings.walkSpeed;
        components.agent.stoppingDistance = stats.attackRadius;

        //healthOrb.outer = healthOrbT;
        //healthOrb.inner = healthOrbT.GetChild(0);
        //healthOrb.outerMat = healthOrb.outer.GetComponent<Renderer>().material;
        //healthOrb.innerMat = healthOrb.inner.GetComponent<Renderer>().material;

        //if(weapon)
        //    weapon.entity = this;
        //if(shield)
        //    shield.entity = this;
    }

    public override void Update()
    {
        base.Update();

        time = Time.time; //temp.

        if(!stats.isDead)
        {
            Collider[] hitPlayers = Physics.OverlapSphere(transform.position, stats.detectionRadius, layerMasks.player);
            float closestPlayerSqrDist = 0f;

            if(hitPlayers.Length > 0)
            {
                closestPlayerSqrDist = (hitPlayers[0].transform.position - transform.position).sqrMagnitude;

                //Vector3 heading = hitPlayers[0].transform.position - transform.position;
                //heading.y = 0;
                //transform.forward = heading;

                LookAt(hitPlayers[0].transform.position);

                components.agent.destination = hitPlayers[0].transform.position;

                //If not attacking AND there is a Player outside attackingRadius
                if(!stats.isAttacking && closestPlayerSqrDist > stats.attackRadius * stats.attackRadius)
                {
                    components.agent.speed = settings.walkSpeed;
                    components.anim.SetBool("IsWalking", true);
                }
                else
                {
                    components.agent.speed = 0f;
                    components.anim.SetBool("IsWalking", false);
                }

                //If Player is within attackingRadius
                if(closestPlayerSqrDist <= stats.attackRadius * stats.attackRadius)
                {
                    //If this is first frame that a Player is in the attackingRadius of this Enemy, set nextTimeAttack to prevent Enemy from always attacking the first frame they become in range of a Player (this should help make less predictable behaviour)
                    if(!stats.isInAttackingRange)
                    {
                        Debug.LogError("Not in attacking range");


                        nextTimeAttack = Time.time + UnityEngine.Random.Range(minAttackDelay, maxAttackDelay);

                        stats.isInAttackingRange = true;
                    }

                    if(!stats.isAttacking && Time.time - nextTimeAttack >= 0) //If not attacking and enough time has passed to attack again
                    {
                        Debug.LogError("Attacking");


                        Block(false);
                        blockDelay.paused = true;
                        Attack();


                        nextTimeAttack = Time.time + UnityEngine.Random.Range(minAttackDelay, maxAttackDelay);
                    }
                    else if(!stats.isAttacking) //Else if waiting to attack
                    {
                        Debug.LogError("Waiting to attack " + (Time.time - nextTimeAttack));

                        //TODO: Fix random blocking; still need to block for random intervals and use ActionDelay for attack delays, but somewhat works b/c has seemingly random blocking delay after attacking, allowing Player some time to get an attack in
                        if(blockDelay.paused) //The first frame after finishing attack, force the next random occurance to be set and resume blockDelay
                        {
                            blockDelay.SetNextRandomOccurance(true);


                            blockDelay.paused = false;
                        }

                        //If not yet blocking AND enough time passed in order to start blocking
                        if(!stats.isBlocking && blockDelay.ready)
                        {
                            Debug.LogError("Blocking");


                            //TODO: Make Enemy block at random times for random intervals (See comment in Enemy.cs before Start())
                            Block(true); //stats.isBlocking is set to true in Block()
                        }
                        else if(!stats.isBlocking)
                        {
                            Debug.LogError("Waiting to block (and waiting to atack)");
                        }

                        if(duelOpponent)
                        {
                            //Make Enemy walk in random directions (eventually will be able to walk in the opposite direction of locked on Player for random intervals (needs callback/reference to locked on Player for this))
                            { }
                            //Make Enemy walk in opposite direction of duel opponent for a random duration
                            {

                            }

                        }


                        blockDelay.SetNextRandomOccurance(); //AFTER BLOCKING; Sets the next occurance time if not already waiting for next occurance
                    }
                }
                else
                {
                    stats.isInAttackingRange = false;
                }
            }
            else
            {
                components.agent.speed = 0f;
                components.anim.SetBool("IsWalking", false);
            }
        }
    }

    public override void Attack()
    {
        base.Attack();

        components.anim.SetBool("IsWalking", false);

        stats.isAttacking = true;
        components.anim.Play("Primary");
    }

    public override void OnPostAttack()
    {
        base.OnPostAttack();


        stats.isAttacking = false;
    }

    //public bool CanTakeDamage()
    //{
    //    return 
    //}

    ///// <summary>
    ///// Optional parameter in order to calculate how much damage to take when blocking
    ///// </summary>
    ///// <param name="damage"></param>
    ///// <returns></returns>
    //public bool TakeDamage(float damage = 0f)
    //{
    //    nextTimeHit = Time.time + stats.getHitRestTime;


    //    if(stats.health > 0)
    //    {
    //        stats.health -= damage;
    //    }

    //    UpdateHealth();

    //    if(stats.health <= 0)
    //    {
    //        Die();
    //    }

    //    return true;
    //}

    //public float CalculateDamage(Item item) //Eventually pass in Weapon type (need to make) that has reference to the character that is hitting this Enemy
    //{
    //    Entity e = item.GetEntity();

    //    if(e is TPSPlayer)
    //        return (item.GetStrength() * ((TPSPlayer)e).stats.strength) / (stats.toughness * (stats.isBlocking ? shield.GetToughness() : 1f));
    //    else
    //        return 0f; //Temp.
    //}

    //public void Damage(Entity entity, float damage)
    //{
    //    entity.TakeDamage(damage);
    //}

    //private void UpdateHealth()
    //{
    //    Color healthColor = Color.Lerp(Color.red, Color.green, Mathf.Clamp01(stats.health / stats.maxHealth));

    //    //Option 1
    //    //foreach(Material m in healthMaterials)
    //    //    m.SetColor("_EmissionColor", healthColor); //Make selection color based on selected target's health

    //    //Option 2
    //    healthOrb.outerMat.SetColor("_EmissionColor", healthColor);
    //    healthOrb.innerMat.SetColor("_Color", healthColor);
    //    healthOrb.inner.localScale = Mathf.Clamp01(stats.health / stats.maxHealth) * Vector3.one;

    //    //Or have both scale (so constant out ring) OR have inside scale faster than outside so has cool effect
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, stats.bodyHalfExtents * 2f);
        Gizmos.DrawWireSphere(transform.position, stats.attackRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stats.detectionRadius);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, stats.minRadius);

        if(duelOpponent)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, duelOpponent.transform.position);
            Gizmos.color = Color.blue;
            //Gizmos.DrawLine(transform.position, randDir * 15f);
            Gizmos.DrawLine(duelOpponent.transform.position, duelOpponent.GetDuelOpponentMoveDirection() * 15f);
        }
    }
}
