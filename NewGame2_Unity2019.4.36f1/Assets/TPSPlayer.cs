using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

/// <summary>
/// Modified from Brackey's tutorial on YouTube ("THIRD PERSON MOVEMENT in Unity": https://www.youtube.com/watch?v=4HpC--2iowE)
/// </summary>
public class TPSPlayer : Entity
{
    [Serializable]
    public class Controls
    {
        public enum KeyCodePress
        {
            ONKEY, ONKEYDOWN, ONKEYUP
        }


        public TPSPlayer player;

        //Tuples won't be in Inspector
        public (string strafe, string frontBack) movementAxes = ("Horizontal", "Vertical");
        public (float strafe, float frontBack) movementInput;

        public (string x, string y) lookAxes = ("Mouse X", "Mouse Y");
        public (float x, float y) lookInput;

        //Can make KeyCode tuple with main KeyCode and alt KeyCode
        public (KeyCode main, KeyCode alt) jump = (KeyCode.Space, KeyCode.None);
        public (KeyCode main, KeyCode alt) run = (KeyCode.LeftShift, KeyCode.RightShift);
        public (KeyCode main, KeyCode alt) lockOn = (KeyCode.Q, KeyCode.None);
        public (KeyCode main, KeyCode alt) primary = (KeyCode.Mouse0, KeyCode.None);
        public (KeyCode main, KeyCode alt) secondary = (KeyCode.Mouse1, KeyCode.None);
        public (KeyCode main, KeyCode alt) tertiary = (KeyCode.Mouse2, KeyCode.None);
        public (KeyCode main, KeyCode alt) quaternary = (KeyCode.Mouse3, KeyCode.None);
        public (KeyCode main, KeyCode alt) action5 = (KeyCode.Mouse4, KeyCode.None);
        public (KeyCode main, KeyCode alt) pause = (KeyCode.Escape, KeyCode.None);


        public void Initialize(TPSPlayer player)
        {
            this.player = player;
        }

        public void GetInput()
        {
            movementInput.strafe = Input.GetAxisRaw(movementAxes.strafe); //Maybe also do * Time.deltaTime
            movementInput.frontBack = Input.GetAxisRaw(movementAxes.frontBack); //Maybe also do * Time.deltaTime

            lookInput.x = Input.GetAxis(lookAxes.x);
            lookInput.y = Input.GetAxis(lookAxes.y);
            //lookInput.x = lookInput.y = 0; //Setting them to zero?
        }

        public bool Jump()
        {
            return GetKeyDown(jump);
        }

        public bool Run()
        {
            return GetKey(run);
        }

        public bool LockOn(KeyCodePress keyCodePress = KeyCodePress.ONKEYDOWN)
        {
            return GetKeyPress(lockOn, keyCodePress);
        }

        public bool Primary(KeyCodePress keyCodePress = KeyCodePress.ONKEY)
        {
            return GetKeyPress(primary, keyCodePress);
        }

        public bool Secondary(KeyCodePress keyCodePress = KeyCodePress.ONKEY)
        {
            return GetKeyPress(secondary, keyCodePress);
        }

        public bool Tertiary(KeyCodePress keyCodePress = KeyCodePress.ONKEY)
        {
            return GetKeyPress(tertiary, keyCodePress);
        }

        public bool Quaternary(KeyCodePress keyCodePress = KeyCodePress.ONKEY)
        {
            return GetKeyPress(quaternary, keyCodePress);
        }

        public bool Action5(KeyCodePress keyCodePress = KeyCodePress.ONKEY)
        {
            return GetKeyPress(action5, keyCodePress);
        }

        public bool Pause(KeyCodePress keyCodePress = KeyCodePress.ONKEYDOWN)
        {
            return GetKeyPress(pause, keyCodePress);
        }

        public static bool GetKeyPress((KeyCode main, KeyCode alt) keyCodeTuple, KeyCodePress keyCodePress = KeyCodePress.ONKEY)
        {
            if(keyCodePress == KeyCodePress.ONKEY)
                return GetKey(keyCodeTuple);
            else if(keyCodePress == KeyCodePress.ONKEYDOWN)
                return GetKeyDown(keyCodeTuple);
            else
                return GetKeyUp(keyCodeTuple);
        }

        public static bool GetKey((KeyCode main, KeyCode alt) keyCodeTuple)
        {
            return Input.GetKey(keyCodeTuple.main) || Input.GetKey(keyCodeTuple.alt);
        }

        public static bool GetKeyDown((KeyCode main, KeyCode alt) keyCodeTuple)
        {
            return Input.GetKeyDown(keyCodeTuple.main) || Input.GetKeyDown(keyCodeTuple.alt);
        }

        public static bool GetKeyUp((KeyCode main, KeyCode alt) keyCodeTuple)
        {
            return Input.GetKeyUp(keyCodeTuple.main) || Input.GetKeyUp(keyCodeTuple.alt);
        }
    }

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

    //[Serializable]
    //public new class Stats : Entity.Stats
    //{
    //    //Extents for checking collisions (half extents, not full extents)
    //    public Vector3 selectHalfExtents = new Vector3(1f, 1f, 5f);

    //    public bool isPaused;
    //}

    //public class Components
    //{
    //    //public Rigidbody rb;
    //    public CharacterController characterController;
    //    public Animator anim;
    //    public Collider collider;


    //    public void GetComponents(TPSPlayerTemplate player)
    //    {
    //        //rb = player.GetComponent<Rigidbody>();
    //        characterController = player.GetComponent<CharacterController>();
    //        anim = player.GetComponent<Animator>();
    //        collider = player.GetComponent<Collider>();
    //    }
    //}

    //[Serializable]
    //public class Stats
    //{
    //    public float maxHealth = 100f;
    //    public float stamina = 5f;
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

    public Controls controls = new Controls();
    public Settings settings = new Settings();
    //public Components components = new Components();
    //public Stats stats = new Stats();
    //public new Stats stats = new Stats();

    public new Camera camera;
    public Transform cameraRigT;
    public (Camera camera, CinemachineFreeLook normal, CinemachineFreeLook lockOn, CinemachineFreeLook aim) cameraRig;
    public CinemachineFreeLook newTPScam; //temp.
    private ApplyImageEffect grayScale;
    //public Transform groundCheckOrigin;
    //public LayerMask groundLayerMask;
    //public Transform healthOrbT;
    //(Transform outer, Material outerMat, Transform inner, Material innerMat) healthOrb;

    public Entity selectedEntity;
    public Transform selectedHorse;
    
    public Vector3 dir;
    private Vector3 duelMoveDirection;
    public Vector3 lockOnForwardHeading;
    public Vector3 lockOnRightheading;

    public Inventory inventory;

    //public Item weapon;
    //public Item shield;

    //Temp./Holder Variables
    //public static int EverythingMask = ~(0 << 32);
    //public LayerMask weaponLayer;
    float xRot; //Used for rotating character around Y Axis when aiming
    //Collider col;
    Vector3 velocityFromGravity = Vector3.zero;
    float turnSmoothVelocity;
    //Quaternion quaternionZero = new Quaternion();
    //float nextTimeHit;
    float nextTimeCheckVelocity;
    Vector3 lastPos;
    Enemy lastAttacker;
    public Mesh cubeMesh;
    public Collider[] hitEntities;
    public Transform arrow;
    public RectTransform r;

    public bool isFocusedOn; //temp.

    public float cinematicBarsHeight = 0.125f;
    public float cinematicBarsSpeed = 1f;

    public PlayableDirector playableDirector;
    public bool play;
    public bool activateCinematicBars;
    public CinemachineVirtualCamera cinematicCamera;
    public PlayableAsset[] playablesArray;
    public Dictionary<string, PlayableAsset> playables = new Dictionary<string, PlayableAsset>();


    /*
     * New Hierarchy For Characters:
     * 
     * BasicCharacter (Common for all Characters)
     *      Rig (has Animator; GFX; also contains HealthOrb)
     *      GroundCheckOrigin (probably should just convert to Vector3 variable)
     *      
     * -------
     * 
     * Player
     *      Rig (GFX; also contains HealthOrb)
     *      GroundCheckOrigin (probably should just convert to Vector3 variable)
     *      CameraRig (Only for Players)
     *      UI (HUD for Player, etc.)
     */


    public override void Start()
    {
        base.Start();


        gameObject.tag = "Player";
        gameObject.layer = LayerMask.NameToLayer("Player");

        controls.Initialize(this);
        //components.GetComponents(this);

        Cursor.lockState = CursorLockMode.Locked;

        //healthOrb.outer = healthOrbT;
        //healthOrb.inner = healthOrbT.GetChild(0);
        //healthOrb.outerMat = healthOrb.outer.GetComponent<Renderer>().material;
        //healthOrb.innerMat = healthOrb.inner.GetComponent<Renderer>().material;

        //ShowHealthOrb(HealthOrbState.OPEN);

        //if(weapon)
        //    weapon.entity = this;
        //if(shield)
        //    shield.entity = this;

        cameraRig.camera = camera;
        cameraRig.normal = cameraRigT.GetChild(0).GetComponent<CinemachineFreeLook>(); //Normal CinemachineFreeLook camera controller is first child of cameraRigT Transform
        cameraRig.lockOn = cameraRigT.GetChild(1).GetComponent<CinemachineFreeLook>(); //Lock On CinemachineFreeLook camera controller is second child of cameraRigT Transform
        cameraRig.aim = cameraRigT.GetChild(2).GetComponent<CinemachineFreeLook>();

        grayScale = camera.GetComponent<ApplyImageEffect>();
        grayScale.enabled = false;

        foreach(PlayableAsset p in playablesArray)
        {
            playables.Add(p.name, p);
        }

        //Ensure the normal Camera is the last one enabled (so that the cinematic bars Camera does not cover the normal Camera)
        camera.enabled = false;
        camera.enabled = true;

        Debug.LogError(playables["Intro"]);
    }

    public override void Update()
    {
        if(!stats.isPaused)
        {
            base.Update();
            

            if(!stats.isDead)
            {
                controls.GetInput();

                Move(controls.movementInput.strafe, controls.movementInput.frontBack);

                Collider hitItemPickupCol = collisions.GetCollisionByTag(camera.transform.position + camera.transform.forward * stats.selectHalfExtents.z, stats.selectHalfExtents, "ItemPickup", camera.transform.rotation, QueryTriggerInteraction.Collide);

                if(hitItemPickupCol)
                {
                    ItemPickup hitItemPickup = hitItemPickupCol.GetComponent<ItemPickup>();

                    if(hitItemPickup)
                    {
                        if(Input.GetKeyDown(KeyCode.E))
                        {
                            inventory.AddToInventory(hitItemPickup.inventoryItem);
                        }
                    }
                }

                //Change to happen only when aim button pressed
                if(stats.isAiming)
                {
                    cameraRig.aim.Priority = 10;
                    cameraRig.normal.Priority = 0;
                    cameraRig.lockOn.Priority = 0;

                    float y = components.anim.GetFloat("Y");
                    y += controls.lookInput.y * Time.deltaTime * stats.aimSpeedY;
                    y = Mathf.Clamp(y, -1, 1);
                    components.anim.SetFloat("Y", y);
                    Debug.LogError(components.anim.transform.name);

                    cameraRig.aim.m_YAxis.Value = MapNeg1ToPos1To01(y * -1f);

                    xRot = controls.lookInput.x * stats.aimSpeedX;
                    transform.Rotate(Vector3.up, xRot * Time.deltaTime);

                    //Vector3 pos = aimAtT.localPosition;

                    //pos.y += controls.lookInput.y;
                    //pos.y = Mathf.Clamp(pos.y, minAim, maxAim);

                    //aimAtT.localPosition = pos;
                }

                if(controls.LockOn())
                {
                    stats.isLockedOn = !stats.isLockedOn;

                    if(stats.isLockedOn)
                    {
                        //TODO: Fix this part where Player presses lock on to EXIT the duel (problem now is that Player must be aiming at the current duelOpponent in order to exit the duel, making it impossible to exit a duel if Player gets too 
                        //far away from duelOpponent; fix this by checking if there is a duelOpponent (only if there is no duelOpponent, should you check the currently selected Entity (selectedEntity)) and if there is, exit the duel)
                        if(selectedEntity)
                        {
                            EnterLockOn(true);
                        }
                    }
                    else
                    {
                        //If pressed lock on after just locking on to something (prevents this CinemachineFreeLook from snapping to lockOn CinemachineFreeLook's position every time lockOn is pressed and there was no previous target)
                        if(selectedEntity)
                        {
                            EnterLockOn(false);


                            selectedEntity = null;
                        }
                    }
                }

                if(controls.Primary())
                {
                    if(stats.isAiming)
                    {
                        //Do aiming primary action (should be shoot)
                    }
                    else
                    {
                        Attack();
                    }
                }
                if(controls.Secondary() && !components.anim.GetBool("SecondaryIdle"))
                {
                    if(!stats.isAttacking)
                    {
                        if(stats.isAiming)
                        {
                            //Do aiming secondary action (should be zoom, windup (pullback for bow and arrow), etc.)
                        }
                        else
                        {
                            stats.isBlocking = true;
                            components.anim.SetFloat("Secondary", 1f);
                            components.anim.Play("Secondary_Start");
                            components.anim.SetBool("SecondaryIdle", true);
                        }
                    }
                }
                else if(!controls.Secondary())
                {
                    stats.isBlocking = false;
                    components.anim.SetFloat("Secondary", 0f);
                    components.anim.SetBool("SecondaryIdle", false);
                }

                if(Input.GetKeyDown(KeyCode.C))
                {
                    cameraRig.normal.gameObject.SetActive(!cameraRig.normal.gameObject.activeSelf);
                    newTPScam.gameObject.SetActive(!newTPScam.gameObject.activeSelf);
                }

                //Uncomment if want to check Player Layers (but shouldn't unless multiple Players b/c then have to exclude this Player from the returned Colliders)
                /*Collider[]*/
                hitEntities = collisions.GetCollisions(camera.transform.position + camera.transform.forward * stats.selectHalfExtents.z, stats.selectHalfExtents, 1 << LayerMask.NameToLayer("Enemy"),
     camera.transform.rotation);

                if(hitEntities.Length > 0)
                {
                    Entity newSelectedEntity = null;
                    for(int i = 0; i < hitEntities.Length; i++)
                    {
                        Entity entity = hitEntities[i].GetComponent<Entity>();
                        if(!entity.stats.isDead)
                        {
                            newSelectedEntity = entity;
                            break;
                        }
                    }

                    //Entity newSelectedEntity = hitEntities[0].GetComponent<Entity>();

                    if(newSelectedEntity && (selectedEntity && !selectedEntity.Equals(newSelectedEntity) || !selectedEntity)) //If not selecting the same Entity as the frame before
                    {
                        if(selectedEntity)
                            selectedEntity.ShowHealthOrb(HealthOrbState.CLOSE);

                        selectedEntity = newSelectedEntity;

                        if(selectedEntity)
                            selectedEntity.ShowHealthOrb(HealthOrbState.OPEN);
                    }

                }
                else if(selectedEntity) //Else if selecting nothing and last selected Entity is still not null, hide its HealthOrb and set selectedEntity to null
                {
                    selectedEntity.ShowHealthOrb(HealthOrbState.CLOSE);
                    selectedEntity = null;
                }

                //-------------------------

                //Old 2
                //                Collider[] hitHorses = collisions.GetCollisions(camera.transform.position + camera.transform.forward * stats.selectHalfExtents.z, stats.selectHalfExtents, 1 << LayerMask.NameToLayer("Horse"),
                //camera.transform.rotation);

                //                if(hitHorses.Length > 0)
                //                {
                //                    Transform newSelectedHorse = null;
                //                    for(int i = 0; i < hitHorses.Length; i++)
                //                    {
                //                        Transform horse = hitHorses[i].transform;
                //                        if(horse)
                //                        {
                //                            newSelectedHorse = horse;
                //                            break;
                //                        }
                //                    }

                //                    if(newSelectedHorse && (selectedHorse && !selectedHorse.Equals(newSelectedHorse) || !selectedHorse)) //If not selecting the same Entity as the frame before
                //                    {
                //                        //if(selectedHorse)
                //                        //    selectedHorse.ShowHealthOrb(HealthOrbState.CLOSE);

                //                        selectedHorse = newSelectedHorse;

                //                        //if(selectedHorse)
                //                        //    selectedHorse.ShowHealthOrb(HealthOrbState.OPEN);
                //                    }

                //                }
                //                else if(selectedHorse) //Else if selecting nothing and last selected Entity is still not null, hide its HealthOrb and set selectedHorse to null
                //                {
                //                    //selectedHorse.ShowHealthOrb(HealthOrbState.CLOSE);
                //                    selectedHorse = null;
                //                }

                //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                if(controls.Secondary())
                {
                    if(!selectedHorse)
                    {
                        Collider[] hitHorses = collisions.GetCollisions(camera.transform.position + camera.transform.forward * stats.selectHalfExtents.z, stats.selectHalfExtents, 1 << LayerMask.NameToLayer("Horse"),
                        camera.transform.rotation);

                        if(hitHorses.Length > 0)
                        {
                            Transform newSelectedHorse = null;
                            for(int i = 0; i < hitHorses.Length; i++)
                            {
                                Transform horse = hitHorses[i].transform;
                                if(horse)
                                {
                                    newSelectedHorse = horse;
                                    break;
                                }
                            }

                            if(newSelectedHorse && (selectedHorse && !selectedHorse.Equals(newSelectedHorse) || !selectedHorse)) //If not selecting the same Horse as the frame before
                            {
                                //if(selectedHorse)
                                //    selectedHorse.ShowHealthOrb(HealthOrbState.CLOSE);

                                selectedHorse = newSelectedHorse;
                                isFocusedOn = true;

                                //if(selectedHorse)
                                //    selectedHorse.ShowHealthOrb(HealthOrbState.OPEN);
                            }

                        }
                        else if(selectedHorse) //Else if selecting nothing and last selected Horse is still not null, hide its HealthOrb and set selectedHorse to null
                        {
                            //selectedHorse.ShowHealthOrb(HealthOrbState.CLOSE);
                            selectedHorse = null;
                        }
                    }
                }
                else
                {
                    selectedHorse = null;
                    isFocusedOn = false;
                }

            //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                //           Collider[] hitHorses = collisions.GetCollisions(camera.transform.position + camera.transform.forward * stats.selectHalfExtents.z, stats.selectHalfExtents, 1 << LayerMask.NameToLayer("Horse"),
                //camera.transform.rotation);

                //           if(hitHorses.Length > 0)
                //           {
                //               if(controls.Secondary()/* && !selectedHorse*/) //temp.
                //               {
                //                   if(!selectedHorse)
                //                   {
                //                       Transform hitHorse = hitHorses[0].transform;

                //                       if(hitHorse && !hitHorse.Equals(selectedHorse)) //If not selecting same Horse as frame before
                //                       {
                //                           selectedHorse = hitHorse;

                //                           //FIX!!!!!!!!; Doesn't set isLockedOn to true so Player does not move relative to the locked on target so the camera moves away from target when Player moves
                //                           //FIX!!!!!!!!; Doesn't set isLockedOn to true so Player does not move relative to the locked on target so the camera moves away from target when Player moves
                //                           //FIX!!!!!!!!; Doesn't set isLockedOn to true so Player does not move relative to the locked on target so the camera moves away from target when Player moves
                //                           //FIX!!!!!!!!; Doesn't set isLockedOn to true so Player does not move relative to the locked on target so the camera moves away from target when Player moves
                //                           //FIX!!!!!!!!; Doesn't set isLockedOn to true so Player does not move relative to the locked on target so the camera moves away from target when Player moves

                //                           LockOn(selectedHorse, true);
                //                       }
                //                   }
                //               }
                //               else
                //               {
                //                   if(selectedHorse)
                //                       LockOn(selectedHorse, false);

                //                   selectedHorse = null;
                //               }
                //           }

                //Debug.LogError("Health Orb State: " + healthOrb.state + "; last: " + healthOrb.lastState);

                //if(healthOrb.state != HealthOrbState.OPEN)
                //ShowHealthOrb(HealthOrbState.OPEN);
            }

            //if(controls.Secondary())
            //    Debug.Log("2");
            //if(controls.Tertiary())
            //    Debug.Log("3");
            //if(controls.Quaternary())
            //    Debug.Log("4");
            //if(controls.Action5())
            //    Debug.Log("5");

            //if(!stats.isDead)
            //{
            //    if(col = collisions.GetWeaponCollision())
            //    {
            //        if(!stats.isGettingHit)
            //        {
            //            Entity attackingEntity = null;
            //            Item weapon = col.GetComponent<Item>();
            //            if(weapon)
            //                attackingEntity = weapon.GetEntity();

            //            if(attackingEntity && attackingEntity.stats.isAttacking)
            //            {
            //                TakeDamage(attackingEntity, CalculateDamage(attackingEntity, weapon));

            //                if(!stats.isDead) //Entity can call Die() in TakeDamage() if their health becomes <= 0 in that frame when TakeDamage() is called (so don't play knock back animation after Entity calls Die())
            //                {
            //                    if(stats.isBlocking)
            //                        components.anim.Play("BlockHit");
            //                    else
            //                        components.anim.Play("Jump_Start");
            //                }

            //                stats.isGettingHit = true;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        stats.isGettingHit = false;
            //    }
            //}

            if(Input.GetKey(KeyCode.V) || activateCinematicBars)
            {
                Rect rect = camera.rect;

                rect.height = Mathf.Lerp(rect.height, 1f - cinematicBarsHeight * 2f, Time.deltaTime * cinematicBarsSpeed);
                rect.y = (1 - rect.height) / 2f;

                camera.rect = rect;
            }
            else
            {
                Rect rect = camera.rect;

                rect.height = Mathf.Lerp(rect.height, 1f, Time.deltaTime * cinematicBarsSpeed);
                rect.y = (1 - rect.height) / 2f;

                camera.rect = rect;
            }

            if(Time.time - nextTimeCheckVelocity >= 0)
            {
                stats.velocity = (transform.position - lastPos) / settings.velocityCheckTimeDelta;

                lastPos = transform.position;


                nextTimeCheckVelocity = Time.time + settings.velocityCheckTimeDelta;
            }
        }

        if(play)
        {
            cameraRig.normal.Priority = 0;
            cinematicCamera.Priority = 10;

            activateCinematicBars = true;
            playableDirector.Play();
            
            //playableDirector.played += <Action> //try adding an action to this and see if it calls that action when the animation is finished playing

        }

        if(controls.Pause())
        {
            stats.isPaused = !stats.isPaused;

            if(stats.isPaused)
            {
                Time.timeScale = 0f;
                grayScale.enabled = true;
            }
            else
            {
                Time.timeScale = 1f;
                grayScale.enabled = false;
            }
        }
    }

    private void Move(float localXInput, float localZInput) //Either use these parameters or remove them
    {
        stats.isGrounded = collisions.GroundCheck();

        components.anim.SetBool("IsGrounded", stats.isGrounded);

        if(stats.isGrounded) //With the current ground check range, jumps are actually reset the very next frame after a jump, so if Stats.maxJumps == 2, Player can actually jump 3 times before landing
            stats.jumps = stats.maxJumps;

        //TODO: Fix this b/c if jumped on a non-Obstacle surface, velocityFromGravity keeps building up and once Player steps off surface, they fall very fast (possibly clipping through ground and dying), but the stats.velocity.sqrMagnitude <= 0.01f 
        //check doesn't always work b/c it messes up the jump (sometimes once the Player jumps, velocity is close to 0 but it is reset by this check, preventing Player from finishing the jump)
        if((stats.isGrounded && velocityFromGravity.y < 0f)/* || stats.velocity.sqrMagnitude <= 0.01f*/) //If grounded and velocityFromGravity is negative OR velocity is very small (i.e. is on top of non-Obstacle surface so isGrounded is false)
        {
            velocityFromGravity.x = velocityFromGravity.z = 0f;
            velocityFromGravity.y = -2f; //Set it to -2f instead of 0 b/c isGrounded can become true slightly before player actually hits the ground (depending on sphere check radius)
        }

        //Jumping
        if(stats.jumps > 0)
        {
            if(controls.Jump())
            {
                if(stats.jumps > 0)
                {
                    Jump(settings.jumpHeight);


                    stats.jumps--;
                }
            }
        }

        if(stats.isGrounded || (!stats.isGrounded && settings.airControl))
        {
            //Vector3 dir;

            if(stats.isLockedOn || isFocusedOn/*temp.*//*&& selectedEntity*/)
            {
                ///*Vector3*/
                //lockOnForwardHeading = selectedEntity.transform.position - transform.position;
                //lockOnForwardHeading.y = 0;
                //lockOnForwardHeading.Normalize();
                //transform.forward = lockOnForwardHeading; //Look at lock on target BEFORE setting dir b/c dir uses Transform.right (which can change when looking at something)

                //Old
                //lockOnForwardHeading = LookAt(selectedEntity.transform.position);

                //dir = ((transform.right * controls.movementInput.strafe) + (lockOnForwardHeading * controls.movementInput.frontBack)).normalized;
                //duelMoveDirection = dir;

                //New
                if(selectedHorse)
                {
                    lockOnForwardHeading = LookAt(selectedHorse.position);

                    dir = ((transform.right * controls.movementInput.strafe) + (lockOnForwardHeading * controls.movementInput.frontBack)).normalized;
                    duelMoveDirection = dir;
                }
                else if(selectedEntity)
                {
                    lockOnForwardHeading = LookAt(selectedEntity.transform.position);

                    dir = ((transform.right * controls.movementInput.strafe) + (lockOnForwardHeading * controls.movementInput.frontBack)).normalized;
                    duelMoveDirection = dir;
                }
            }
            else
            {
                dir = ((Vector3.right * controls.movementInput.strafe) + (Vector3.forward * controls.movementInput.frontBack)).normalized;
                duelMoveDirection = Vector3.zero;
            }

            if(dir.sqrMagnitude >= 0.1f * 0.1f)
            {
                Vector3 moveDir;
                float targetAngle, angle;
                if(!stats.isLockedOn && !isFocusedOn/*temp.*/ /*&& !selectedEntity*/) //Used to be || (???????????????????????)
                {
                    /*float*/
                    targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
                    /*float*/ angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, settings.turnSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    /*Vector3*/ moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; //Multiply by this Vector to turn the rotation into a direction
                }
                else
                {
                    Debug.LogError("Line 807: Here is where the messed up movement happends when looking at an enemy (but duel movement gets messed up if you remove the if-else conditions)", this);
                    moveDir = dir;
                }

                if(!stats.isAttacking) //Maybe make it so it also doesn't take input while attacking (instead of just not applying movement if isAttacking)
                {
                    components.characterController.Move(GetSpeed() * moveDir.normalized * Time.deltaTime);

                    if(controls.Run() && stats.isGrounded)
                    {
                        components.anim.SetBool("IsRunning", true);
                        components.anim.SetBool("IsWalking", false);
                    }
                    else if(stats.isGrounded)
                    {
                        components.anim.SetBool("IsWalking", true);
                        components.anim.SetBool("IsRunning", false);
                    }
                    //Else if trying to walk/run in air
                    else
                    {
                        components.anim.SetBool("IsWalking", false);
                        components.anim.SetBool("IsRunning", false);
                    }
                }
            }
            else
            {
                components.anim.SetBool("IsWalking", false);
                components.anim.SetBool("IsRunning", false);
            }

            //Apply Gravity
            velocityFromGravity.y += settings.gravity * Time.deltaTime;
            components.characterController.Move(velocityFromGravity * Time.deltaTime); //Multiply velocityFromGravity by Time.deltaTime again b/c deltaY = 1/2 * g * t^2
        }
    }

    public override Vector3 GetDuelOpponentMoveDirection()
    {
        return duelMoveDirection;
    }

    //private Enemy GetEnemyFromColliderOrParent(Collider col)
    //{
    //    Enemy e = col.GetComponent<Enemy>();

    //    if(!e)
    //        e = col.GetComponentInParent<Enemy>();

    //    return e;
    //}

    private float GetSpeed()
    {
        if(controls.Run())
            return settings.runSpeed;
        else
            return settings.walkSpeed;
    }

    public void Jump(float jumpHeight)
    {
        velocityFromGravity.y = GetJumpForce(jumpHeight); //Set y component INSTEAD of adding to it
        components.anim.SetTrigger("Jump");
    }

    public void Jump(float jumpHeight, Vector3 direction)
    {
        velocityFromGravity = GetJumpForce(jumpHeight) * direction; //Set velocityFromGravity INSTEAD of adding to it
        components.anim.SetTrigger("Jump");
    }

    private float GetJumpForce(float desiredJumpHeight)
    {
        //v = sqrt(height * -2 * gravity)

        return Mathf.Sqrt(desiredJumpHeight * -2f * settings.gravity);
    }

    /// <summary>
    /// Modified from Alok Singhal's answer at stackoverflow (https://stackoverflow.com/questions/5731863/mapping-a-numeric-range-onto-another). Maps a value from the range -1, 1 to the range 0, 1
    /// </summary>
    /// <returns></returns>
    private float MapNeg1ToPos1To01(float input)
    {
        //float slope = (output_end - output_start) / (input_end - input_start);
        //float output = output_start + slope * (input - input_start);

        float minIN = -1, maxIN = 1, minOUT = 0, maxOUT = 1;

        float slope = (maxOUT - minOUT) / (maxIN - minIN);
        float output = minOUT + slope * (input - minIN);

        return output;
    }

    private void EnterLockOn(bool enter)
    {
        stats.isLockedOn = enter;

        if(enter)
        {
            OnPreDuel(selectedEntity);

            cameraRig.lockOn.Priority = 10;
            cameraRig.normal.Priority = 0;
            cameraRig.aim.Priority = 0;

            cameraRig.lockOn.LookAt = selectedEntity.transform;
            selectedEntity.ShowHealthOrb(HealthOrbState.OPEN);
        }
        else
        {
            OnPostDuel(selectedEntity);

            cameraRig.normal.Priority = 10;
            cameraRig.lockOn.Priority = 0;
            cameraRig.aim.Priority = 0;

            cameraRig.normal.ForceCameraPosition(cameraRig.lockOn.transform.position, cameraRig.lockOn.transform.rotation);
            selectedEntity.ShowHealthOrb(HealthOrbState.CLOSE);
        }
    }

    public void FocusOn(Transform target, bool enter)
    {
        if(target)
        {
            if(enter)
            {
                stats.isLockedOn = true;

                cameraRig.lockOn.Priority = 10;
                cameraRig.normal.Priority = 0;
                cameraRig.aim.Priority = 0;

                cameraRig.lockOn.LookAt = target;
                //selectedEntity.ShowHealthOrb(HealthOrbState.OPEN); //Maybe check if target is Entity and then show/hide HealthOrb
            }
            else
            {
                stats.isLockedOn = false;

                cameraRig.normal.Priority = 10;
                cameraRig.lockOn.Priority = 0;
                cameraRig.aim.Priority = 0;

                cameraRig.normal.ForceCameraPosition(cameraRig.lockOn.transform.position, cameraRig.lockOn.transform.rotation);
                //selectedEntity.ShowHealthOrb(HealthOrbState.CLOSE); //Maybe check if target is Entity and then show/hide HealthOrb
            }
        }
    }

    public override void Attack()
    {
        base.Attack();

        components.anim.SetBool("IsRunning", false);
        components.anim.SetBool("IsWalking", false);

        stats.isAttacking = true;
        components.anim.Play("Primary");
    }

    public override void OnPostAttack()
    {
        base.OnPostAttack();


        stats.isAttacking = false;
    }

    //public bool CanGetHitBy(Enemy enemy)
    //{
    //    return !enemy.Equals(lastAttacker) || Time.time - nextTimeHit > 0f;
    //}

    //public float CalculateDamage(Entity attackingEntity, Item attackingItem)
    //{
    //    //temp. going to make it so don't need to cast
    //    if(attackingEntity is TPSPlayer)
    //        return (attackingItem.GetStrength() * ((TPSPlayer)attackingEntity).stats.strength) / (stats.toughness * (stats.isBlocking ? shield.stats.toughness : 1f));
    //    else if(attackingEntity is Enemy)
    //        return (attackingItem.GetStrength() * ((Enemy)attackingEntity).stats.strength) / (stats.toughness * (stats.isBlocking ? shield.stats.toughness : 1f));
    //    else
    //        return 0f; //Temp.
    //}

    ///// <summary>
    ///// If both Enemy and float parameters are given, Enemy.stats.damage will override the float parameter
    ///// </summary>
    ///// <param name="e"></param>
    ///// <param name="damage"></param>
    //public bool TakeDamage(Enemy e = null, float damage = 0f)
    //{
    //    if(e)
    //    {
    //        if(!e.Equals(lastAttacker))
    //        {
    //            lastAttacker = e;
    //            nextTimeHit = Time.time + stats.getHitRestTime;

    //            damage = e.stats.damage;
    //        }
    //        else if(Time.time - nextTimeHit > 0f) //If enough time passed in order to get hit by same Enemy (which is e)
    //        {
    //            damage = e.stats.damage;
    //        }
    //        else
    //            return false;
    //    }
    //    else
    //    {
    //        lastAttacker = null;
    //        nextTimeHit = 0;
    //    }

    //    if(stats.health > 0)
    //    {
    //        stats.health -= damage;
    //    }

    //    if(stats.health <= 0)
    //    {
    //        Die();
    //    }


    //    return true;
    //}

    //public void Damage(Enemy e, float damage)
    //{
    //    e.TakeDamage(damage);
    //}

    ///// <summary>
    ///// Optional parameter in order to calculate how much damage to take when blocking
    ///// </summary>
    ///// <param name="damage"></param>
    ///// <returns></returns>
    //public bool TakeDamage(float damage = 0f)
    //{
    //    //See other TakeDamage() commented out above
    //    //See other TakeDamage() commented out above
    //    //See other TakeDamage() commented out above
    //    //See other TakeDamage() commented out above
    //    //See other TakeDamage() commented out above

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

    //public void ShowHealthOrb(bool show)
    //{
    //    healthOrb.outer.gameObject.SetActive(show);
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

    public override void OnKilledEntity(Entity entity)
    {
        base.OnKilledEntity(entity);


        //If killed the Entity that was currently being locked on to, exit lock on
        if(stats.isLockedOn && entity.Equals(selectedEntity))
        {
            EnterLockOn(false);
        }
    }

    public void PlayCutscene(string playableName)
    {
        cameraRig.normal.Priority = 0;
        cinematicCamera.Priority = 10;

        activateCinematicBars = true;

        playableDirector.playableAsset = playables[playableName];
        playableDirector.Play();
    }

    public void OnExitCutscene()
    {
        cinematicCamera.Priority = 0;
        cameraRig.normal.Priority = 10;

        activateCinematicBars = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position, stats.bodyHalfExtents * 2f);

        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, transform.position + velocityFromGravity);
        Gizmos.DrawWireCube(groundCheckOrigin.position, stats.groundCheckHalfExtents * 2f);

        Gizmos.color = Color.green;

        if(cubeMesh)
            Gizmos.DrawWireMesh(cubeMesh, camera.transform.position + camera.transform.forward * stats.selectHalfExtents.z, camera.transform.rotation, stats.selectHalfExtents * 2f);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position, transform.position + dir * 50f);
        //Gizmos.DrawLine(transform.position, transform.position + lockOnForwardHeading * 15f);

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, transform.position + lockOnRightheading * 15f);

        Gizmos.DrawLine(arrow.position, arrow.position + arrow.forward * 500f);
    }
}
