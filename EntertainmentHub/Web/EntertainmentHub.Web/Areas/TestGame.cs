using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class Referee : MultiReferee
{
    public static int PLAYER_COUNT = 3;
    static int GAME_VERSION = 3;

    static bool SPAWN_WRECK = false;
    static int LOOTER_COUNT = 3;
    static bool REAPER_SKILL_ACTIVE = true;
    static bool DESTROYER_SKILL_ACTIVE = true;
    static bool DOOF_SKILL_ACTIVE = true;

    static double MAP_RADIUS = 6000.0;
    static int TANKERS_BY_PLAYER;
    static int TANKERS_BY_PLAYER_MIN = 1;
    static int TANKERS_BY_PLAYER_MAX = 3;

    static double WATERTOWN_RADIUS = 3000.0;

    static int TANKER_THRUST = 500;
    static double TANKER_EMPTY_MASS = 2.5;
    static double TANKER_MASS_BY_WATER = 0.5;
    static double TANKER_FRICTION = 0.40;
    static double TANKER_RADIUS_BASE = 400.0;
    static double TANKER_RADIUS_BY_SIZE = 50.0;
    static int TANKER_EMPTY_WATER = 1;
    static int TANKER_MIN_SIZE = 4;
    static int TANKER_MAX_SIZE = 10;
    static double TANKER_MIN_RADIUS = TANKER_RADIUS_BASE + TANKER_RADIUS_BY_SIZE * TANKER_MIN_SIZE;
    static double TANKER_MAX_RADIUS = TANKER_RADIUS_BASE + TANKER_RADIUS_BY_SIZE * TANKER_MAX_SIZE;
    static double TANKER_SPAWN_RADIUS = 8000.0;
    static int TANKER_START_THRUST = 2000;

    static int MAX_THRUST = 300;
    static int MAX_RAGE = 300;
    static int WIN_SCORE = 50;

    static double REAPER_MASS = 0.5;
    static double REAPER_FRICTION = 0.20;
    static int REAPER_SKILL_DURATION = 3;
    static int REAPER_SKILL_COST = 30;
    static int REAPER_SKILL_ORDER = 0;
    static double REAPER_SKILL_RANGE = 2000.0;
    static double REAPER_SKILL_RADIUS = 1000.0;
    static double REAPER_SKILL_MASS_BONUS = 10.0;

    static double DESTROYER_MASS = 1.5;
    static double DESTROYER_FRICTION = 0.30;
    static int DESTROYER_SKILL_DURATION = 1;
    static int DESTROYER_SKILL_COST = 60;
    static int DESTROYER_SKILL_ORDER = 2;
    static double DESTROYER_SKILL_RANGE = 2000.0;
    static double DESTROYER_SKILL_RADIUS = 1000.0;
    static int DESTROYER_NITRO_GRENADE_POWER = 1000;

    static double DOOF_MASS = 1.0;
    static double DOOF_FRICTION = 0.25;
    static double DOOF_RAGE_COEF = 1.0 / 100.0;
    static int DOOF_SKILL_DURATION = 3;
    static int DOOF_SKILL_COST = 30;
    static int DOOF_SKILL_ORDER = 1;
    static double DOOF_SKILL_RANGE = 2000.0;
    static double DOOF_SKILL_RADIUS = 1000.0;

    static double LOOTER_RADIUS = 400.0;
    const int LOOTER_REAPER = 0;
    const int LOOTER_DESTROYER = 1;
    const int LOOTER_DOOF = 2;

    static int TYPE_TANKER = 3;
    static int TYPE_WRECK = 4;
    static int TYPE_REAPER_SKILL_EFFECT = 5;
    static int TYPE_DOOF_SKILL_EFFECT = 6;
    static int TYPE_DESTROYER_SKILL_EFFECT = 7;

    static double EPSILON = 0.00001;
    static double MIN_IMPULSE = 30.0;
    static double IMPULSE_COEFF = 0.5;

    // Global first free id for all elements on the map 
    static int GLOBAL_ID = 0;

    // Center of the map
    static readonly Point WATERTOWN = new Point(0, 0);

    // The null collision 
    static readonly Collision NULL_COLLISION = new Collision(1.0 + EPSILON);

    public class Point
    {
        public double x;
        public double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double Distance(Point p)
        {
            return Math.Sqrt((this.x - p.x) * (this.x - p.x) + (this.y - p.y) * (this.y - p.y));
        }

        // Move the point to x and y
        public void move(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        // Move the point to an other point for a given distance
        public void moveTo(Point p, double dist)
        {
            double d = Distance(p);

            if (d < EPSILON)
            {
                return;
            }

            double dx = p.x - x;
            double dy = p.y - y;
            double coef = dist / d;

            this.x += dx * coef;
            this.y += dy * coef;
        }

        public bool isInRange(Point p, double range)
        {
            return p != this && Distance(p) <= range;
        }

        public override int GetHashCode()
        {
            const int prime = 31;

            int result = 1;

            long temp;
            temp = BitConverter.DoubleToInt64Bits(x);
            result = prime * result + (int)((ulong)temp ^ ((ulong)temp >> 32));
            temp = BitConverter.DoubleToInt64Bits(y);
            result = prime * result + (int)((ulong)temp ^ ((ulong)temp >> 32));
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            Point other = (Point)obj;
            if (BitConverter.DoubleToInt64Bits(x) != BitConverter.DoubleToInt64Bits(other.x)) return false;
            if (BitConverter.DoubleToInt64Bits(y) != BitConverter.DoubleToInt64Bits(other.y)) return false;
            return true;
        }
    }

    public class Wreck : Point
    {
        public int id;
        public double radius;
        public int water;
        public bool known;
        public Player player;

        public Wreck(double x, double y, int water, double radius) : base(x, y)
        {
            id = GLOBAL_ID++;

            this.radius = radius;
            this.water = water;
        }

        public virtual string getFrameId() => id + "@" + water;

        public string toFrameData()
        {
            if (known)
                return getFrameId();

            known = true;

            return join(getFrameId(), Math.Round(x), Math.Round(y), 0, 0, TYPE_WRECK, radius);
        }

        // Reaper harvesting
        public bool harvest(List<Player> players, List<SkillEffect> skillEffects)
        {
            players.ForEach(p =>
            {
                if (isInRange(p.getReaper(), radius) && !p.getReaper().isInDoofSkill(skillEffects))
                {
                    p.score += 1;
                    water -= 1;
                }
            });

            return water > 0;
        }
    }

    abstract public class Unit : Point
    {
        public int type;
        public int id;
        public double vx;
        public double vy;
        public double radius;
        public double mass;
        public double friction;
        public bool known;

        public Unit(int type, double x, double y) : base(x, y)
        {
            id = GLOBAL_ID++;
            this.type = type;

            vx = 0.0;
            vy = 0.0;

            known = false;
        }

        public void move(double t)
        {
            x += vx * t;
            y += vy * t;
        }

        public double speed()
        {
            return Math.Sqrt(vx * vx + vy * vy);
        }

        public override int GetHashCode() => 31 + id;

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (obj == null || GetType() != GetType())
                return false;

            Unit other = (Unit)obj;

            return id == other.id;
        }

        public virtual string getFrameId() => id.ToString();

        public string toFrameData()
        {
            if (known)
                return string.Join(getFrameId(), Math.Round(x), Math.Round(y), Math.Round(vx), Math.Round(vy));

            known = true;

            return string.Join(getFrameId(), Math.Round(x), Math.Round(y), Math.Round(vx), Math.Round(vy), type, Math.Round(radius));
        }

        public void thrust(Point p, int power)
        {
            double distance = Distance(p);

            // Avoid a division by zero
            if (Math.Abs(distance) <= EPSILON)
                return;

            double coef = (((double)power) / mass) / distance;
            vx += (p.x - x) * coef;
            vy += (p.y - y) * coef;
        }

        public bool isInDoofSkill(IEnumerable<SkillEffect> skillEffects)
        {
            return skillEffects.Any(s => s is DoofSkillEffect && isInRange(s, s.radius + radius));
        }

        public void adjust(IEnumerable<SkillEffect> skillEffects)
        {
            x = round(x);
            y = round(y);

            if (isInDoofSkill(skillEffects))
            {
                // No friction if we are in a doof skill effect
                vx = round(vx);
                vy = round(vy);
            }
            else
            {
                vx = round(vx * (1.0 - friction));
                vy = round(vy * (1.0 - friction));
            }
        }

        // Search the next collision with the map border
        public virtual Collision getCollision()
        {
            // Check instant collision
            if (Distance(WATERTOWN) + radius >= MAP_RADIUS)
            {
                return new Collision(0.0, this);
            }

            // We are not moving, we can't reach the map border
            if (vx == 0.0 && vy == 0.0)
            {
                return NULL_COLLISION;
            }

            // Search collision with map border
            // Resolving: sqrt((x + t*vx)^2 + (y + t*vy)^2) = MAP_RADIUS - radius <=> t^2*(vx^2 + vy^2) + t*2*(x*vx + y*vy) + x^2 + y^2 - (MAP_RADIUS - radius)^2 = 0
            // at^2 + bt + c = 0;
            // a = vx^2 + vy^2
            // b = 2*(x*vx + y*vy)
            // c = x^2 + y^2 - (MAP_RADIUS - radius)^2

            double a = vx * vx + vy * vy;

            if (a <= 0.0)
            {
                return NULL_COLLISION;
            }

            double b = 2.0 * (x * vx + y * vy);
            double c = x * x + y * y - (MAP_RADIUS - radius) * (MAP_RADIUS - radius);
            double delta = b * b - 4.0 * a * c;

            if (delta <= 0.0)
            {
                return NULL_COLLISION;
            }

            double t = (-b + Math.Sqrt(delta)) / (2.0 * a);

            if (t <= 0.0)
            {
                return NULL_COLLISION;
            }

            return new Collision(t, this);
        }

        // Search the next collision with an other unit
        public virtual Collision getCollision(Unit u)
        {
            // Check instant collision
            if (Distance(u) <= radius + u.radius)
            {
                return new Collision(0.0, this, u);
            }

            // Both units are motionless
            if (vx == 0.0 && vy == 0.0 && u.vx == 0.0 && u.vy == 0.0)
            {
                return NULL_COLLISION;
            }

            // Change referencial
            // Unit u is not at point (0, 0) with a speed vector of (0, 0)
            double x2 = x - u.x;
            double y2 = y - u.y;
            double r2 = radius + u.radius;
            double vx2 = vx - u.vx;
            double vy2 = vy - u.vy;

            // Resolving: sqrt((x + t*vx)^2 + (y + t*vy)^2) = radius <=> t^2*(vx^2 + vy^2) + t*2*(x*vx + y*vy) + x^2 + y^2 - radius^2 = 0
            // at^2 + bt + c = 0;
            // a = vx^2 + vy^2
            // b = 2*(x*vx + y*vy)
            // c = x^2 + y^2 - radius^2 

            double a = vx2 * vx2 + vy2 * vy2;

            if (a <= 0.0)
            {
                return NULL_COLLISION;
            }

            double b = 2.0 * (x2 * vx2 + y2 * vy2);
            double c = x2 * x2 + y2 * y2 - r2 * r2;
            double delta = b * b - 4.0 * a * c;

            if (delta < 0.0)
            {
                return NULL_COLLISION;
            }

            double t = (-b - Math.Sqrt(delta)) / (2.0 * a);

            if (t <= 0.0)
            {
                return NULL_COLLISION;
            }

            return new Collision(t, this, u);
        }

        // Bounce between 2 units
        public void bounce(Unit u)
        {
            double mcoeff = (mass + u.mass) / (mass * u.mass);
            double nx = x - u.x;
            double ny = y - u.y;
            double nxnysquare = nx * nx + ny * ny;
            double dvx = vx - u.vx;
            double dvy = vy - u.vy;
            double product = (nx * dvx + ny * dvy) / (nxnysquare * mcoeff);
            double fx = nx * product;
            double fy = ny * product;
            double m1c = 1.0 / mass;
            double m2c = 1.0 / u.mass;

            vx -= fx * m1c;
            vy -= fy * m1c;
            u.vx += fx * m2c;
            u.vy += fy * m2c;

            fx = fx * IMPULSE_COEFF;
            fy = fy * IMPULSE_COEFF;

            // Normalize vector at min or max impulse
            double impulse = Math.Sqrt(fx * fx + fy * fy);
            double coeff = 1.0;
            if (impulse > EPSILON && impulse < MIN_IMPULSE)
            {
                coeff = MIN_IMPULSE / impulse;
            }

            fx = fx * coeff;
            fy = fy * coeff;

            vx -= fx * m1c;
            vy -= fy * m1c;
            u.vx += fx * m2c;
            u.vy += fy * m2c;

            double diff = (Distance(u) - radius - u.radius) / 2.0;
            if (diff <= 0.0)
            {
                // Unit overlapping. Fix positions.
                moveTo(u, diff - EPSILON);
                u.moveTo(this, diff - EPSILON);
            }
        }

        // Bounce with the map border
        public void bounce()
        {
            double mcoeff = 1.0 / mass;
            double nxnysquare = x * x + y * y;
            double product = (x * vx + y * vy) / (nxnysquare * mcoeff);
            double fx = x * product;
            double fy = y * product;

            vx -= fx * mcoeff;
            vy -= fy * mcoeff;

            fx = fx * IMPULSE_COEFF;
            fy = fy * IMPULSE_COEFF;

            // Normalize vector at min or max impulse
            double impulse = Math.Sqrt(fx * fx + fy * fy);
            double coeff = 1.0;
            if (impulse > EPSILON && impulse < MIN_IMPULSE)
            {
                coeff = MIN_IMPULSE / impulse;
            }

            fx = fx * coeff;
            fy = fy * coeff;
            vx -= fx * mcoeff;
            vy -= fy * mcoeff;

            double diff = Distance(WATERTOWN) + radius - MAP_RADIUS;
            if (diff >= 0.0)
            {
                // Unit still outside of the map, reposition it
                moveTo(WATERTOWN, diff + EPSILON);
            }
        }

        public virtual int getExtraInput()
        {
            return -1;
        }

        public virtual int getExtraInput2()
        {
            return -1;
        }

        public virtual int getPlayerIndex()
        {
            return -1;
        }
    }

    public class Tanker : Unit
    {
        public int water;
        public int size;
        public Player player;
        public bool killed;

        public Tanker(int size, Player player) : base(TYPE_TANKER, 0.0, 0.0)
        {
            this.player = player;
            this.size = size;

            water = TANKER_EMPTY_WATER;
            mass = TANKER_EMPTY_MASS + TANKER_MASS_BY_WATER * water;
            friction = TANKER_FRICTION;
            radius = TANKER_RADIUS_BASE + TANKER_RADIUS_BY_SIZE * size;
        }

        public override string getFrameId()
        {
            return id + "@" + water;
        }

        public Wreck die()
        {
            // Don't spawn a wreck if our center is outside of the map
            if (Distance(WATERTOWN) >= MAP_RADIUS)
            {
                return null;
            }

            return new Wreck(round(x), round(y), water, radius);
        }

        public bool isFull()
        {
            return water >= size;
        }

        public void play()
        {
            // Try to leave the map
            if (isFull())
                thrust(WATERTOWN, -TANKER_THRUST);

            // Try to reach watertown
            else if (Distance(WATERTOWN) > WATERTOWN_RADIUS)
                thrust(WATERTOWN, TANKER_THRUST);
        }

        // Tankers can go outside of the map
        public override Collision getCollision() => NULL_COLLISION;

        public override int getExtraInput()
        {
            return water;
        }

        public override int getExtraInput2()
        {
            return size;
        }
    }

    abstract public class Looter : Unit
    {
        public int skillCost;
        public double skillRange;
        public bool skillActive;

        public Player player;

        public Point wantedThrustTarget;
        public int wantedThrustPower;

        public string message;
        public Action attempt;
        public SkillResult skillResult;

        public Looter(int type, Player player, double x, double y) : base(type, x, y)
        {
            this.player = player;
            radius = LOOTER_RADIUS;
        }

        public SkillEffect skill(Point p)
        {
            if (player.rage < skillCost)
                throw new NoRageException();
            if (Distance(p) > skillRange)
                throw new TooFarException();

            player.rage -= skillCost;
            return skillImpl(p);
        }

        public new string toFrameData()
        {
            if (known)
                return base.toFrameData();

            return join(base.toFrameData(), player.index);
        }

        public override int getPlayerIndex()
        {
            return player.index;
        }

        public abstract SkillEffect skillImpl(Point p);

        public void setWantedThrust(Point target, int power)
        {
            if (power < 0)
                power = 0;

            wantedThrustTarget = target;
            wantedThrustPower = Math.Min(power, MAX_THRUST);
        }

        public void reset()
        {
            message = null;
            attempt = 0;
            skillResult = null;
            wantedThrustTarget = null;
        }
    }

    public class Reaper : Looter
    {
        public Reaper(Player player, double x, double y) :
            base(LOOTER_REAPER, player, x, y)
        {
            mass = REAPER_MASS;
            friction = REAPER_FRICTION;
            skillCost = REAPER_SKILL_COST;
            skillRange = REAPER_SKILL_RANGE;
            skillActive = REAPER_SKILL_ACTIVE;
        }

        public override SkillEffect skillImpl(Point p) =>
            new ReaperSkillEffect(TYPE_REAPER_SKILL_EFFECT, p.x, p.y, REAPER_SKILL_RADIUS, REAPER_SKILL_DURATION, REAPER_SKILL_ORDER, this);
    }

    public class Destroyer : Looter
    {
        public Destroyer(Player player, double x, double y) :
            base(LOOTER_DESTROYER, player, x, y)
        {
            mass = DESTROYER_MASS;
            friction = DESTROYER_FRICTION;
            skillCost = DESTROYER_SKILL_COST;
            skillRange = DESTROYER_SKILL_RANGE;
            skillActive = DESTROYER_SKILL_ACTIVE;
        }

        public override SkillEffect skillImpl(Point p) =>
            new DestroyerSkillEffect(TYPE_DESTROYER_SKILL_EFFECT, p.x, p.y, DESTROYER_SKILL_RADIUS, DESTROYER_SKILL_DURATION,
                    DESTROYER_SKILL_ORDER, this);
    }

    public class Doof : Looter
    {
        public Doof(Player player, double x, double y) :
            base(LOOTER_DOOF, player, x, y)
        {
            mass = DOOF_MASS;
            friction = DOOF_FRICTION;
            skillCost = DOOF_SKILL_COST;
            skillRange = DOOF_SKILL_RANGE;
            skillActive = DOOF_SKILL_ACTIVE;
        }

        public override SkillEffect skillImpl(Point p) =>
            new DoofSkillEffect(TYPE_DOOF_SKILL_EFFECT, p.x, p.y, DOOF_SKILL_RADIUS, DOOF_SKILL_DURATION, DOOF_SKILL_ORDER, this);

        // With flame effects! Yeah!
        public int sing()
        {
            return (int)Math.Floor(speed() * DOOF_RAGE_COEF);
        }
    }

    public class Player
    {
        public int score;
        public int index;
        public int rage;
        public Looter[] looters;
        public bool dead;
        public Queue<TankerSpawn> tankers;

        public Player(int index)
        {
            this.index = index;
            looters = new Looter[LOOTER_COUNT];
        }

        public void kill()
        {
            dead = true;
        }

        public Reaper getReaper()
        {
            return (Reaper)looters[LOOTER_REAPER];
        }

        public Destroyer getDestroyer()
        {
            return (Destroyer)looters[LOOTER_DESTROYER];
        }

        public Doof getDoof()
        {
            return (Doof)looters[LOOTER_DOOF];
        }
    }

    public class TankerSpawn
    {
        public int size;
        public double angle;

        public TankerSpawn(int size, double angle)
        {
            this.size = size;
            this.angle = angle;
        }
    }

    public class Collision
    {
        public double t;
        public Unit a;
        public Unit b;

        public Collision(double t) : this(t, null, null) { }

        public Collision(double t, Unit a) : this(t, a, null) { }

        public Collision(double t, Unit a, Unit b)
        {
            this.t = t;
            this.a = a;
            this.b = b;
        }

        public Tanker dead()
        {
            if (a is Destroyer && b is Tanker && b.mass < REAPER_SKILL_MASS_BONUS)
            {
                return (Tanker)b;
            }

            if (b is Destroyer && a is Tanker && a.mass < REAPER_SKILL_MASS_BONUS)
            {
                return (Tanker)a;
            }

            return null;
        }
    }

    abstract public class SkillEffect : Point
    {
        public int id;
        public int type;
        public double radius;
        public int duration;
        public int order;
        public bool known;
        public Looter looter;

        public SkillEffect(int type, double x, double y, double radius, int duration, int order, Looter looter) : base(x, y)
        {
            id = GLOBAL_ID++;

            this.type = type;
            this.radius = radius;
            this.duration = duration;
            this.looter = looter;
            this.order = order;
        }

        public void apply(List<Unit> units)
        {
            duration -= 1;
            applyImpl(units.Where(u => isInRange(u, radius + u.radius)).ToList());
        }

        public string toFrameData()
        {
            if (known)
                return id.ToString();

            known = true;

            return join(id, Math.Round(x), Math.Round(y), looter.id, 0, type, Math.Round(radius));
        }

        public abstract void applyImpl(List<Unit> units);

        public override int GetHashCode() => base.GetHashCode();

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;

            SkillEffect other = (SkillEffect)obj;
            return id != other.id;
        }
    }

    public class ReaperSkillEffect : SkillEffect
    {
        public ReaperSkillEffect(int type, double x, double y, double radius, int duration, int order, Reaper reaper) :
            base(type, x, y, radius, duration, order, reaper)
        { }

        // Increase mass
        public override void applyImpl(List<Unit> units)
        {
            units.ForEach(u => u.mass += REAPER_SKILL_MASS_BONUS);
        }
    }

    public class DestroyerSkillEffect : SkillEffect
    {
        public DestroyerSkillEffect(int type, double x, double y, double radius, int duration, int order, Destroyer destroyer) :
            base(type, x, y, radius, duration, order, destroyer)
        { }

        // Push units
        public override void applyImpl(List<Unit> units)
        {
            units.ForEach(u => u.thrust(this, -DESTROYER_NITRO_GRENADE_POWER));
        }
    }

    public class DoofSkillEffect : SkillEffect
    {
        public DoofSkillEffect(int type, double x, double y, double radius, int duration, int order, Doof doof) :
            base(type, x, y, radius, duration, order, doof)
        { }

        // Nothing to do now
        public override void applyImpl(List<Unit> units) { }
    }

    // Round away from zero
    public static int round(double x)
    {
        int s = x < 0 ? -1 : 1;
        return s * (int)Math.Round(s * x);
    }

    // Join multiple object into a space separated string
    public static string join(params object[] values)
    {
        return string.Join(" ", values);
    }

    public int seed;
    public int playerCount;
    public List<Unit> units;
    public List<Looter> looters;
    public List<Tanker> tankers;
    public List<Tanker> deadTankers;
    public List<Wreck> wrecks;
    public List<IEnumerable<Unit>> unitsByType;
    public List<Player> players;
    public List<string> frameData;
    public List<SkillEffect> skillEffects;

    void spawnTanker(Player player)
    {
        TankerSpawn spawn = player.tankers.Dequeue();

        double angle = (player.index + spawn.angle) * Math.PI * 2.0 / ((double)playerCount);

        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);

        if (SPAWN_WRECK)
        {
            // Spawn a wreck directly
            Wreck wreck = new Wreck(cos * WATERTOWN_RADIUS, sin * WATERTOWN_RADIUS, spawn.size,
                    TANKER_RADIUS_BASE + spawn.size * TANKER_RADIUS_BY_SIZE);
            wreck.player = player;

            wrecks.Add(wreck);

            return;
        }

        Tanker tanker = new Tanker(spawn.size, player);

        double distance = TANKER_SPAWN_RADIUS + tanker.radius;

        bool safe = false;
        while (!safe)
        {
            tanker.move(cos * distance, sin * distance);
            safe = units.All(u => tanker.Distance(u) > tanker.radius + u.radius);
            distance += TANKER_MIN_RADIUS;
        }

        tanker.thrust(WATERTOWN, TANKER_START_THRUST);

        units.Add(tanker);
        tankers.Add(tanker);
    }

    Looter createLooter(int type, Player player, double x, double y)
    {
        switch (type)
        {
            case LOOTER_REAPER:
                return new Reaper(player, x, y);
            case LOOTER_DESTROYER:
                return new Destroyer(player, x, y);
            case LOOTER_DOOF:
                return new Doof(player, x, y);
        }

        // Not supposed to happen
        return null;
    }

    void newFrame(double t)
    {
        frameData.Add("#" + t.ToString("F5"));
    }

    void addToFrame(Wreck w)
    {
        frameData.Add(w.toFrameData());
    }

    void addToFrame(Unit u)
    {
        frameData.Add(u.toFrameData());
    }

    void addToFrame(SkillEffect s)
    {
        frameData.Add(s.toFrameData());
    }

    void addDeadToFrame(SkillEffect s)
    {
        frameData.Add(join(s.toFrameData(), "d"));
    }

    void addDeadToFrame(Unit u)
    {
        frameData.Add(join(u.toFrameData(), "d"));
    }

    void addDeadToFrame(Wreck w)
    {
        frameData.Add(join(w.toFrameData(), "d"));
    }

    void snapshot()
    {
        unitsByType.ForEach(units =>
        {
            frameData.AddRange(units.Select(u => u.toFrameData()).ToList());
        });

        frameData.AddRange(wrecks.Select(w => w.toFrameData()).ToList());
        frameData.AddRange(skillEffects.Select(s => s.toFrameData()).ToList());
    }

    protected override void initReferee(int playerCount, Properties prop)
    {
        this.playerCount = playerCount;

        try
        {
            seed = int.Parse(prop.getProperty("seed", (new Random().Next().ToString())));
        }
        catch (FormatException)
        {
            seed = new Random().Next();
        }

        Random random = new Random(seed);

        TANKERS_BY_PLAYER = TANKERS_BY_PLAYER_MIN + random.Next(TANKERS_BY_PLAYER_MAX - TANKERS_BY_PLAYER_MIN + 1);

        units = new List<Unit>();
        looters = new List<Looter>();
        tankers = new List<Tanker>();
        deadTankers = new List<Tanker>();
        wrecks = new List<Wreck>();
        players = new List<Player>();

        unitsByType = new List<IEnumerable<Unit>>();
        unitsByType.Add(looters);
        unitsByType.Add(tankers);

        frameData = new List<string>();

        skillEffects = new SortedSet<SkillEffect>(Comparer<SkillEffect>.Create((a, b) =>
        {
            int order = a.order - b.order;

            if (order != 0)
            {
                return order;
            }

            return a.id - b.id;
        })).ToList();

        // Create players
        for (int i = 0; i < playerCount; ++i)
        {
            Player player = new Player(i);
            players.Add(player);
        }

        // Generate the map
        Queue<TankerSpawn> queue = new Queue<TankerSpawn>();
        for (int i = 0; i < 500; ++i)
        {
            queue.Enqueue(new TankerSpawn(TANKER_MIN_SIZE + random.Next(TANKER_MAX_SIZE - TANKER_MIN_SIZE),
                    random.NextDouble()));
        }
        players.ForEach(p => p.tankers = new Queue<TankerSpawn>(queue));

        // Create looters
        foreach (var player in players)
        {
            for (int i = 0; i < LOOTER_COUNT; ++i)
            {
                Looter looter = createLooter(i, player, 0, 0);
                player.looters[i] = looter;
                units.Add(looter);
                looters.Add(looter);
            }
        }

        // Random spawns for looters
        bool finished = false;
        while (!finished)
        {
            finished = true;

            for (int i = 0; i < LOOTER_COUNT && finished; ++i)
            {
                double distance = random.NextDouble() * (MAP_RADIUS - LOOTER_RADIUS);
                double angle = random.NextDouble();

                foreach (var player in players)
                {
                    double looterAngle = (player.index + angle) * (Math.PI * 2.0 / ((double)playerCount));
                    double cos = Math.Cos(looterAngle);
                    double sin = Math.Sin(looterAngle);

                    Looter looter = player.looters[i];
                    looter.move(cos * distance, sin * distance);

                    // If the looter touch a looter, reset everyone and try again
                    if (units.Any(u => u != looter && looter.Distance(u) <= looter.radius + u.radius))
                    {
                        finished = false;
                        looters.ForEach(l => l.move(0.0, 0.0));
                        break;
                    }
                }
            }
        }

        // Spawn start tankers
        for (int j = 0; j < TANKERS_BY_PLAYER; ++j)
        {
            foreach (var player in players)
            {
                spawnTanker(player);
            }
        }

        adjust();
        newFrame(1.0);
        snapshot();
    }

    protected override Properties getConfiguration()
    {
        Properties properties = new Properties();

        properties.putProperty("seed", seed.ToString());

        return properties;
    }

    protected override string[] getInitInputForPlayer(int playerIdx)
    {
        List<string> lines = new List<string>();

        // No init input

        return lines.ToArray();
    }

    protected override void prepare(int round)
    {
        frameData.Clear();
        looters.ForEach(x => x.reset());
    }

    public int getPlayerId(int id, int forId)
    {
        // This method can be called with id=-1 because of the default player for units
        if (id < 0)
            return id;

        if (id == forId)
            return 0;

        if (id < forId)
            return id + 1;

        return id;
    }

    protected override string[] getInputForPlayer(int roundNumber, int playerIdx)
    {
        List<object> lines = new List<object>();

        // Scores
        // My score is always first
        lines.Add(players[playerIdx].score);
        for (int i = 0; i < playerCount; ++i)
        {
            if (i != playerIdx)
            {
                lines.Add(players[i].score);
            }
        }

        // Rages
        // My rage is always first
        lines.Add(players[playerIdx].rage);
        for (int i = 0; i < playerCount; ++i)
        {
            if (i != playerIdx)
            {
                lines.Add(players[i].rage);
            }
        }

        // Units
        List<string> unitsLines = new List<string>();

        // Looters and tankers
        unitsLines.AddRange(units.Select(u => join(
            u.id,
            u.type,
            getPlayerId(u.getPlayerIndex(), playerIdx),
            u.mass,
            round(u.radius),
            round(u.x),
            round(u.y),
            round(u.vx),
            round(u.vy),
            u.getExtraInput(),
            u.getExtraInput2())));
        // Wrecks
        unitsLines.AddRange(wrecks.Select(w => join(w.id, TYPE_WRECK, -1, -1, round(w.radius), round(w.x), round(w.y), 0, 0, w.water, -1)));
        // Skill effects
        unitsLines.AddRange(skillEffects.Select(s => join(s.id, s.type, -1, -1, round(s.radius), round(s.x), round(s.y), 0, 0, s.duration, -1)));

        lines.Add(unitsLines.Count);
        lines.AddRange(unitsLines);

        return lines.Select(x => x.ToString()).ToArray();
    }

    protected override int getExpectedOutputLineCountForPlayer(int playerIdx)
    {
        return 3;
    }

    private static readonly Regex PLAYER_MOVE_PATTERN = new Regex(
        @"^(?<x>-?[0-9]{1,9})\s+(?<y>-?[0-9]{1,9})\s+(?<power>([0-9]{1,9}))?(?:\s+(?<message>.+))?",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture
    );

    private static readonly Regex PLAYER_SKILL_PATTERN = new Regex(
        @"^SKILL\s+(?<x>-?[0-9]{1,9})\s+(?<y>-?[0-9]{1,9})(?:\s+(?<message>.+))?",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture
    );

    private static readonly Regex PLAYER_WAIT_PATTERN = new Regex(
        @"^WAIT(?:\s+(?<message>.+))?",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture
    );

    public class NoRageException : Exception { }

    public class TooFarException : Exception { }

    public enum Action
    {
        NONE = 0,
        SKILL,
        MOVE,
        WAIT
    }

    public class SkillResult
    {
        public static readonly int OK = 0;
        public static readonly int NO_RAGE = 1;
        public static readonly int TOO_FAR = 2;

        public Point target;
        public int code;

        public SkillResult(int x, int y)
        {
            target = new Point(x, y);
            code = OK;
        }

        public int getX()
        {
            return (int)target.x;
        }

        public int getY()
        {
            return (int)target.y;
        }
    }

    protected override void handlePlayerOutput(int frame, int round, int playerIdx, string[] outputs)
    {
        Player player = players[playerIdx];
        string expected = "<x> <y> <power> | SKILL <x> <y> | WAIT";

        for (int i = 0; i < LOOTER_COUNT; ++i)
        {
            string line = outputs[i];

            Match match;

            try
            {
                Looter looter = players[playerIdx].looters[i];

                match = PLAYER_WAIT_PATTERN.Match(line);
                if (match.Success)
                {
                    looter.attempt = Action.WAIT;
                    matchMessage(looter, match);
                    continue;
                }

                match = PLAYER_MOVE_PATTERN.Match(line);
                if (match.Success)
                {
                    looter.attempt = Action.MOVE;
                    int x = int.Parse(match.Groups["x"].Value);
                    int y = int.Parse(match.Groups["y"].Value);
                    int power = int.Parse(match.Groups["power"].Value);

                    looter.setWantedThrust(new Point(x, y), power);
                    matchMessage(looter, match);
                    continue;
                }

                match = PLAYER_SKILL_PATTERN.Match(line);
                if (match.Success)
                {
                    if (!looter.skillActive)
                    {
                        // Don't kill the player for that. Just do a WAIT instead
                        looter.attempt = Action.WAIT;
                        matchMessage(looter, match);
                        continue;
                    }

                    looter.attempt = Action.SKILL;
                    int x = int.Parse(match.Groups["x"].Value);
                    int y = int.Parse(match.Groups["y"].Value);

                    SkillResult result = new SkillResult(x, y);
                    looter.skillResult = result;

                    try
                    {
                        SkillEffect effect = looter.skill(new Point(x, y));
                        skillEffects.Add(effect);
                    }
                    catch (NoRageException)
                    {
                        result.code = SkillResult.NO_RAGE;
                    }
                    catch (TooFarException)
                    {
                        result.code = SkillResult.TOO_FAR;
                    }

                    matchMessage(looter, match);
                    continue;
                }

                throw new InvalidInputException(expected, line);
            }
            catch (InvalidInputException e)
            {
                player.kill();
                throw e;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                player.kill();
                throw new InvalidInputException(expected, line);
            }
        }
    }

    private void matchMessage(Looter looter, Match match)
    {
        looter.message = match.Groups["message"].Value;
        if (looter.message != null && looter.message.Length > 19)
        {
            looter.message = looter.message.Substring(0, 17) + "...";
        }
    }

    // Get the next collision for the current round
    // All units are tested
    Collision getNextCollision()
    {
        Collision result = NULL_COLLISION;

        for (int i = 0; i < units.Count; ++i)
        {
            Unit unit = units[i];

            // Test collision with map border first
            Collision collision = unit.getCollision();

            if (collision.t < result.t)
            {
                result = collision;
            }

            for (int j = i + 1; j < units.Count; ++j)
            {
                collision = unit.getCollision(units[j]);

                if (collision.t < result.t)
                {
                    result = collision;
                }
            }
        }

        return result;
    }

    // Play a collision
    void playCollision(Collision collision)
    {
        if (collision.b == null)
        {
            // Bounce with border
            addToFrame(collision.a);
            collision.a.bounce();
        }
        else
        {
            Tanker dead = collision.dead();

            if (dead != null)
            {
                // A destroyer kill a tanker
                addDeadToFrame(dead);
                deadTankers.Add(dead);
                tankers.Remove(dead);
                units.Remove(dead);

                Wreck wreck = dead.die();

                // If a tanker is too far away, there's no wreck
                if (wreck != null)
                {
                    wrecks.Add(wreck);
                    addToFrame(wreck);
                }
            }
            else
            {
                // Bounce between two units
                addToFrame(collision.a);
                addToFrame(collision.b);
                collision.a.bounce(collision.b);
            }
        }
    }

    protected override void updateGame(int round)
    {
        // Apply skill effects
        foreach (var effect in skillEffects)
            effect.apply(units);

        // Apply thrust for tankers
        foreach (var tanker in tankers)
            tanker.play();

        // Apply wanted thrust for looters
        foreach (var player in players)
            foreach (var looter in player.looters)
                if (looter.wantedThrustTarget != null)
                    looter.thrust(looter.wantedThrustTarget, looter.wantedThrustPower);

        double t = 0.0;

        // Play the round. Stop at each collisions and play it. Reapeat until t > 1.0

        Collision collision = getNextCollision();

        double delta = 0;

        while (collision.t + t <= 1.0)
        {
            delta = collision.t;
            units.ForEach(u => u.move(delta));
            t += collision.t;

            newFrame(t);

            playCollision(collision);

            collision = getNextCollision();
        }

        // No more collision. Move units until the end of the round
        delta = 1.0 - t;
        units.ForEach(u => u.move(delta));

        List<Tanker> tankersToRemove = new List<Tanker>();

        foreach (var tanker in tankers)
        {
            double distance = tanker.Distance(WATERTOWN);
            bool full = tanker.isFull();

            if (distance <= WATERTOWN_RADIUS && !full)
            {
                // A non full tanker in watertown collect some water
                tanker.water += 1;
                tanker.mass += TANKER_MASS_BY_WATER;
            }
            else if (distance >= TANKER_SPAWN_RADIUS + tanker.radius && full)
            {
                // Remove too far away and not full tankers from the game
                tankersToRemove.Add(tanker);
            }
        }

        newFrame(1.0);
        snapshot();

        if (!(tankersToRemove.Count == 0))
        {
            tankersToRemove.ForEach(tanker => addDeadToFrame(tanker));
        }


        units.RemoveAll(x => tankersToRemove.Contains(x));
        tankers.RemoveAll(x => tankersToRemove.Contains(x));
        deadTankers.AddRange(tankersToRemove);

        // Spawn new tankers for each dead tanker during the round
        deadTankers.ForEach(tanker => spawnTanker(tanker.player));
        deadTankers.Clear();

        List<Wreck> deadWrecks = new List<Wreck>();

        // Water collection for reapers
        wrecks = wrecks.Where(w =>
        {
            bool alive = w.harvest(players, skillEffects);

            if (!alive)
            {
                addDeadToFrame(w);
                deadWrecks.Add(w);
            }

            return alive;
        }).ToList();

        if (SPAWN_WRECK)
        {
            deadWrecks.ForEach(w => spawnTanker(w.player));
        }

        // Round values and apply friction
        adjust();

        // Generate rage
        if (LOOTER_COUNT >= 3)
        {
            players.ForEach(p => p.rage = Math.Min(MAX_RAGE, p.rage + p.getDoof().sing()));
        }

        // Restore masses
        units.ForEach(u =>
        {
            while (u.mass >= REAPER_SKILL_MASS_BONUS)
            {
                u.mass -= REAPER_SKILL_MASS_BONUS;
            }
        });

        // Remove dead skill effects
        List<SkillEffect> effectsToRemove = new List<SkillEffect>();
        foreach (var effect in skillEffects)
        {
            if (effect.duration <= 0)
            {
                addDeadToFrame(effect);
                effectsToRemove.Add(effect);
            }
        }

        skillEffects.RemoveAll(x => effectsToRemove.Contains(x));
    }

    protected void adjust()
    {
        units.ForEach(u => u.adjust(skillEffects));
    }

    protected override void populateMessages(Properties p)
    {
        //TODO: write some text
        p.putProperty("Move", "$%d moved looter %d towards (%d,%d) with power %d");
        p.putProperty("SkillFailedTooFar", "$%d %d %d %d");
        p.putProperty("SkillFailedNoRage", "$%d %d %d %d");
        p.putProperty("SkillDestroyer", "$%d %d %d %d");
        p.putProperty("SkillRepear", "$%d %d %d %d");
        p.putProperty("SkillDoof", "$%d %d %d %d");
    }

    protected bool isGameOver()
    {
        if (players.Any(p => p.score >= WIN_SCORE))
        {
            // We got a winner !
            return true;
        }

        List<Player> alive = players.Where(p => !p.dead).ToList();

        if (alive.Count == 1)
        {
            Player survivor = alive[0];

            // If only one player is alive and he got the highest score, end the game now.
            return players.Where(p => !alive.Contains(p)).Any(p => p.score > survivor.score);
        }

        // Everyone is dead. End of the game.
        return alive.Count == 0;
    }

    protected override string[] getInitDataForView()
    {
        List<string> lines = new List<string>();

        lines.Add(playerCount.ToString());
        lines.Add(Math.Round(MAP_RADIUS).ToString());
        lines.Add(Math.Round(WATERTOWN_RADIUS).ToString());
        lines.Add(LOOTER_COUNT.ToString());

        return lines.ToArray();
    }

    protected override string[] getFrameDataForView(int round, int frame, bool keyFrame)
    {
        List<string> lines = new List<string>();

        lines.AddRange(players.Select(p => p.score.ToString()));
        lines.AddRange(players.Select(p => p.rage.ToString()));
        lines.AddRange(looters.Select(l => l.message == null ? "" : l.message));
        lines.AddRange(frameData);

        return lines.ToArray();
    }

    protected override string getGameName()
    {
        return "meanmax";
    }

    protected override string getHeadlineAtGameStartForConsole()
    {
        return "";
    }

    protected override int getMinimumPlayerCount()
    {
        return 1;
    }

    protected override bool showTooltips()
    {
        return true;
    }

    protected override string[] getPlayerActions(int playerIdx, int round)
    {
        return new string[0];
    }

    protected override bool isPlayerDead(int playerIdx)
    {
        return players[playerIdx].dead;
    }

    protected override string getDeathReason(int playerIdx)
    {
        return "$" + playerIdx + ": Eliminated!";
    }

    protected override int getScore(int playerIdx)
    {
        return players[playerIdx].score;
    }

    protected override string[] getGameSummary(int round)
    {
        List<string> lines = new List<string>();

        return lines.ToArray();
    }

    protected override void setPlayerTimeout(int frame, int round, int playerIdx)
    {
        players[playerIdx].kill();
    }

    protected override int getMaxRoundCount(int playerCount)
    {
        return 200;
    }

    protected override bool isTurnBasedGame()
    {
        return false;
    }

    public Referee(TextReader input, TextWriter output, TextWriter error) :
        base(input, output, error)
    { }

    public static void Maina(string[] args)
    {
        switch (GAME_VERSION)
        {
            case 0:
                SPAWN_WRECK = true;
                LOOTER_COUNT = 1;
                REAPER_SKILL_ACTIVE = false;
                DESTROYER_SKILL_ACTIVE = false;
                DOOF_SKILL_ACTIVE = false;
                break;
            case 1:
                LOOTER_COUNT = 2;
                REAPER_SKILL_ACTIVE = false;
                DESTROYER_SKILL_ACTIVE = false;
                DOOF_SKILL_ACTIVE = false;
                break;
            case 2:
                LOOTER_COUNT = 3;
                REAPER_SKILL_ACTIVE = false;
                DOOF_SKILL_ACTIVE = false;
                break;
        }

        // Make sure number parsing works
        CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        new Referee(Console.In, Console.Out, Console.Error);
    }
}

abstract public class MultiReferee : AbstractReferee
{
    private Properties properties;

    public MultiReferee(TextReader input, TextWriter output, TextWriter error) :
        base(input, output, error)
    { }

    protected override void handleInitInputForReferee(int playerCount, string[] init)
    {
        properties = new Properties();

        foreach (var s in init)
        {
            properties.load(new StringReader(s));
        }

        initReferee(playerCount, properties);

        properties = getConfiguration();
    }

    abstract protected void initReferee(int playerCount, Properties prop);

    abstract protected Properties getConfiguration();

    protected override void appendDataToEnd(TextWriter stream)
    {
        stream.WriteLine(format(OutputCommand.UINPUT, properties.Count));

        foreach (DictionaryEntry kvp in properties)
        {
            stream.WriteLine(kvp.Key + "=" + kvp.Value);
        }
    }
}

abstract public class AbstractReferee
{
    private static readonly Regex HEADER_PATTERN = new Regex("\\[\\[(?<cmd>.+)\\] ?(?<lineCount>[0-9]+)\\]");
    private static readonly string LOST_PARSING_REASON_CODE = "INPUT";
    private static readonly string LOST_PARSING_REASON = "Failure: invalid input";

    public class PlayerStatus
    {
        public int id;
        public int score;
        public bool lost, win;
        public string info;
        public string reasonCode;
        public string[] nextInput;

        public PlayerStatus(int id)
        {
            this.id = id;
            lost = false;
            info = null;
        }

        public int getScore()
        {
            return score;
        }

        public bool isLost()
        {
            return lost;
        }

        public string getInfo()
        {
            return info;
        }

        public int getId()
        {
            return id;
        }

        public string getReasonCode()
        {
            return reasonCode;
        }

        public string[] getNextInput()
        {
            return nextInput;
        }
    }

    private static Properties messages = new Properties();

    public class InvalidFormatException : Exception { }

    abstract public class GameException : Exception
    {
        private string reasonCode, tooltipCode;
        private object[] values;

        public GameException(string reasonCode, params object[] values)
        {
            this.reasonCode = reasonCode;
            this.values = values;
        }

        public void setTooltipCode(string tooltipCode)
        {
            this.tooltipCode = tooltipCode;
        }

        public string getReason()
        {
            if (reasonCode != null)
            {
                return translate(reasonCode, values);
            }
            else
            {
                return null;
            }
        }

        public string getReasonCode()
        {
            return reasonCode;
        }

        public string getTooltipCode()
        {
            if (tooltipCode != null)
            {
                return tooltipCode;
            }
            return getReasonCode();
        }
    }

    public class LostException : GameException
    {
        public LostException(string reasonCode, params object[] values) :
            base(reasonCode, values)
        { }
    }

    public class WinException : GameException
    {
        public WinException(string reasonCode, params object[] values) :
            base(reasonCode, values)
        { }
    }

    public class InvalidInputException : GameException
    {
        public InvalidInputException(string expected, string found) :
            base("InvalidInput", expected, found)
        { }
    }

    public class GameOverException : GameException
    {
        public GameOverException(string reasonCode, params object[] values) :
            base(reasonCode, values)
        { }
    }

    public class GameErrorException : Exception
    {
        public GameErrorException(Exception cause) : base(cause.Message, cause) { }
    }

    public enum InputCommand
    {
        INIT,
        GET_GAME_INFO,
        SET_PLAYER_OUTPUT,
        SET_PLAYER_TIMEOUT
    }

    public enum OutputCommand
    {
        VIEW,
        INFOS,
        NEXT_PLAYER_INPUT,
        NEXT_PLAYER_INFO,
        SCORES,
        UINPUT,
        TOOLTIP,
        SUMMARY
    }

    public static string format(object o, int lineCount)
    {
        return string.Format("[[{0}] {1}]", o.ToString(), lineCount);
    }

    public class OutputData : List<string>
    {
        private OutputCommand command;

        public OutputData(OutputCommand command)
        {
            this.command = command;
        }

        public void add(string s)
        {
            if (s != null)
                base.Add(s);
        }

        public void addAll(string[] data)
        {
            if (data != null)
                base.AddRange(data);
        }

        public string tostring()
        {
            var writer = new StringWriter();
            writer.WriteLine(format(command, Count));

            foreach (var line in this)
            {
                writer.WriteLine(line);
            }

            return writer.ToString().Trim();
        }
    }

    public class Tooltip
    {
        public int player;
        public string message;

        public Tooltip(int player, string message)
        {
            this.player = player;
            this.message = message;
        }
    }

    private List<Tooltip> tooltips;
    private int playerCount, alivePlayerCount;
    private int currentPlayer, nextPlayer;
    private PlayerStatus lastPlayer, playerStatus;
    private int frame, round;
    private PlayerStatus[] players;
    private string[] initLines;
    private bool newRound;
    private string reasonCode, reason;

    private TextReader input;
    private TextWriter output;
    private TextWriter error;

    public AbstractReferee(TextReader input, TextWriter output, TextWriter error)
    {
        tooltips = new List<Tooltip>();
        this.input = input;
        this.output = output;
        this.error = error;
        start();
    }

    public string[] ReadInitInputForReferee()
    {
        var initInputForReferee = new List<string>();
        var line = "";
        while ((line = input.ReadLine()) != "")
        {
            initInputForReferee.Add(line);
        }

        return initInputForReferee.ToArray();
    }

    public void start()
    {
        playerCount = Referee.PLAYER_COUNT;

        handleInitInputForReferee(playerCount, ReadInitInputForReferee());

        var s = input;

        try
        {
            alivePlayerCount = playerCount;

            players = new PlayerStatus[playerCount];
            players.Each((x, i) => players[i] = new PlayerStatus(i));

            playerStatus = players[0];
            currentPlayer = nextPlayer = 1;
            round = -1;
            newRound = true;

            while (true)
            {
                lastPlayer = playerStatus;
                playerStatus = selectNextPlayer();

                if (this.round >= getMaxRoundCount(this.playerCount))
                    throw new GameOverException("maxRoundsCountReached");

                if (newRound)
                {
                    prepare(round);

                    if (!isTurnBasedGame())
                        foreach (var player in players)
                            if (!player.lost)
                                player.nextInput = getInputForPlayer(round, player.id);
                            else
                                player.nextInput = null;
                }

                output.WriteLine("###Input " + nextPlayer);

                if (round == 0)
                    foreach (var line in getInitInputForPlayer(nextPlayer))
                        output.WriteLine(line);

                if (isTurnBasedGame())
                    foreach (var line in getInputForPlayer(round, nextPlayer))
                        output.WriteLine(line);
                else
                    foreach (var line in players[nextPlayer].nextInput)
                        output.WriteLine(line);

                int expectedOutputLineCount = getExpectedOutputLineCountForPlayer(nextPlayer);

                output.WriteLine("###Output " + nextPlayer + " " + expectedOutputLineCount);
                try
                {
                    string[] outputs = new string[expectedOutputLineCount];
                    for (int i = 0; i < expectedOutputLineCount; i++)
                        outputs[i] = s.ReadLine();

                    handlePlayerOutput(0, round, nextPlayer, outputs);
                }
                catch (WinException e)
                {
                    playerStatus.score = getScore(nextPlayer);
                    playerStatus.win = true;
                    playerStatus.info = e.getReason();
                    playerStatus.reasonCode = e.getReasonCode();
                    lastPlayer = playerStatus;
                    throw new GameOverException(null);
                }
                catch (GameException e) when (e is LostException || e is InvalidInputException)
                {
                    playerStatus.score = getScore(nextPlayer);
                    playerStatus.lost = true;
                    playerStatus.info = e.getReason();
                    playerStatus.reasonCode = e.getReasonCode();
                    bool otherPlayerIsDead = lastPlayer.lost;
                    lastPlayer = playerStatus;
                    //only end the game, if both players are dead
                    if (otherPlayerIsDead)
                        throw new GameOverException(null);
                }
            }
        }
        catch (GameOverException e)
        {
            newRound = true;
            reasonCode = e.getReasonCode();
            reason = e.getReason();
            error.WriteLine(reason);
            prepare(round);
            updateScores();
            var scores = players.Select(x => x.score).Distinct();

            output.Write("###End ");

            foreach (var score in scores)
            {
                var playersWithScore = players.
                    Where(player => player.score == score).
                    Select(player => player.id.ToString()).
                    Aggregate((a, b) => a + b);

                output.WriteLine(" " + playersWithScore);
            }
        }
    }

    private PlayerStatus selectNextPlayer()
    {
        currentPlayer = nextPlayer;
        newRound = false;

        do
        {
            ++nextPlayer;
            if (nextPlayer >= playerCount)
            {
                nextround();
                nextPlayer = 0;
            }
        } while (this.players[nextPlayer].lost || this.players[nextPlayer].win);
        return players[nextPlayer];
    }

    protected string getColoredReason(bool error, string reason)
    {
        if (error)
        {
            return string.Format("¤RED¤{0}§RED§", reason);
        }
        else
        {
            return string.Format("¤GREEN¤{0}§GREEN§", reason);
        }
    }

    private void dumpView()
    {
        OutputData data = new OutputData(OutputCommand.VIEW);
        string reasonCode = this.reasonCode;
        if (reasonCode == null && playerStatus != null)
            reasonCode = playerStatus.reasonCode;

        if (newRound)
        {
            if (reasonCode != null)
            {
                data.Add(string.Format("KEY_FRAME {0} {1}", frame, reasonCode));
            }
            else
            {
                data.Add(string.Format("KEY_FRAME {0}", frame));
            }
            if (frame == 0)
            {
                data.Add(getGameName());
                data.AddRange(getInitDataForView());
            }
        }
        else
        {
            if (reasonCode != null)
            {
                data.Add(string.Format("INTERMEDIATE_FRAME {0} {1}", frame, reasonCode));
            }
            else
            {
                data.Add(string.Format("INTERMEDIATE_FRAME {0}", frame));
            }
        }

        if (newRound || isTurnBasedGame())
        {
            data.AddRange(getFrameDataForView(round, frame, newRound));
        }

        output.WriteLine(data);
    }

    private void dumpInfos()
    {
        OutputData data = new OutputData(OutputCommand.INFOS);
        if (reason != null && isTurnBasedGame())
        {
            data.Add(getColoredReason(true, reason));
        }
        else
        {
            if (lastPlayer != null)
            {
                string head = lastPlayer.info;
                if (head != null)
                {
                    data.Add(getColoredReason(lastPlayer.lost, head));
                }
                else
                {
                    if (frame > 0)
                    {
                        data.AddRange(getPlayerActions(currentPlayer, newRound ? round - 1 : round));
                    }
                }
            }
        }

        output.WriteLine(data);

        if (newRound && round >= -1 && playerCount > 1)
        {
            OutputData summary = new OutputData(OutputCommand.SUMMARY);

            if (frame == 0)
            {
                string head = getHeadlineAtGameStartForConsole();
                if (head != null)
                {
                    summary.Add(head);
                }
            }

            if (round >= 0)
            {
                summary.AddRange(getGameSummary(round));
            }

            if (!isTurnBasedGame() && reason != null)
            {
                summary.Add(getColoredReason(true, reason));
            }

            output.WriteLine(summary);
        }

        if (tooltips.Count > 0 && (newRound || isTurnBasedGame()))
        {
            data = new OutputData(OutputCommand.TOOLTIP);
            foreach (var tooltip in tooltips)
            {
                data.Add(tooltip.message);
                data.Add(tooltip.player.ToString());
            }

            tooltips.Clear();
            output.WriteLine(data);
        }
    }

    private void dumpNextPlayerInfos()
    {
        OutputData data = new OutputData(OutputCommand.NEXT_PLAYER_INFO);
        data.Add(nextPlayer.ToString());
        data.Add(getExpectedOutputLineCountForPlayer(nextPlayer).ToString());
        if (this.round == 0)
        {
            data.Add(getMillisTimeForFirstround().ToString());
        }
        else
        {
            data.Add(getMillisTimeForround().ToString());
        }
        output.WriteLine(data);
    }

    private void dumpNextPlayerInput()
    {
        OutputData data = new OutputData(OutputCommand.NEXT_PLAYER_INPUT);

        if (round == 0)
        {
            data.AddRange(getInitInputForPlayer(nextPlayer));
        }
        if (isTurnBasedGame())
        {
            players[nextPlayer].nextInput = getInputForPlayer(round, nextPlayer);
        }

        data.AddRange(players[nextPlayer].nextInput);
        output.WriteLine(data);
    }

    protected static string translate(string code, params object[] values)
    {
        try
        {
            return string.Format((string)messages[code], values);
        }
        catch (NullReferenceException)
        {
            return code;
        }
    }

    protected void printError(Object message)
    {
        error.WriteLine(message);
    }

    protected int getMillisTimeForFirstround()
    {
        return 1000;
    }

    protected int getMillisTimeForround()
    {
        return 150;
    }

    protected virtual int getMaxRoundCount(int playerCount)
    {
        return 400;
    }

    private void nextround()
    {
        newRound = true;

        if (++round > 0)
            updateGame(round);

        if (gameOver())
            throw new GameOverException(null);
    }

    protected bool gameIsOver() => gameOver();

    protected bool gameOver() => alivePlayerCount < getMinimumPlayerCount();

    private void updateScores()
    {
        for (int i = 0; i < playerCount; ++i)
        {
            if (!players[i].lost && isPlayerDead(i))
            {
                alivePlayerCount--;
                players[i].lost = true;
                players[i].info = getDeathReason(i);
                addToolTip(i, players[i].info);
            }
            players[i].score = getScore(i);
        }
    }

    protected void addToolTip(int player, string message)
    {
        if (showTooltips())
            tooltips.Add(new Tooltip(player, message));
    }

    /**
     * Add message (key = reasonCode, value = reason)
     *
     * @param p
     */
    protected abstract void populateMessages(Properties p);

    protected virtual bool isTurnBasedGame()
    {
        return false;
    }

    protected abstract void handleInitInputForReferee(int playerCount, string[] init);

    protected abstract string[] getInitDataForView();

    protected abstract string[] getFrameDataForView(int round, int frame, bool keyFrame);

    protected abstract int getExpectedOutputLineCountForPlayer(int playerIdx);

    protected abstract string getGameName();

    protected abstract void appendDataToEnd(TextWriter stream);

    protected abstract void handlePlayerOutput(int frame, int round, int playerIdx, string[] output);

    protected abstract string[] getInitInputForPlayer(int playerIdx);

    protected abstract string[] getInputForPlayer(int round, int playerIdx);

    protected abstract string getHeadlineAtGameStartForConsole();

    protected abstract int getMinimumPlayerCount();

    protected abstract bool showTooltips();

    /**
     * @param round
     * @return scores of all players
     * @throws GameOverException
     */
    protected abstract void updateGame(int round);

    protected abstract void prepare(int round);

    protected abstract bool isPlayerDead(int playerIdx);

    protected abstract string getDeathReason(int playerIdx);

    protected abstract int getScore(int playerIdx);

    protected abstract string[] getGameSummary(int round);

    protected abstract string[] getPlayerActions(int playerIdx, int round);

    protected abstract void setPlayerTimeout(int frame, int round, int playerIdx);
}

public class Properties : Hashtable
{
    public string getProperty(string key, string value) =>
        ContainsKey(key) ? (string)this[key] : value;

    public void putProperty(string key, string value) =>
        base[key] = value;

    internal void load(TextReader textReader)
    {
        string line;

        do
        {
            line = textReader.ReadLine();
            if (line == null)
                break;

            var parts = line.Split(':', '=', '\t', ' ');
            if (parts.Count() > 1)
                putProperty(parts[0], parts[1]);

        } while (line != null);
    }

    public string this[string key]
    {
        get { return getProperty(key, key); }
        set { putProperty(key, value); }
    }
}

public static class Extensions
{
    public static void Each<T>(this IEnumerable<T> list, Action<T, int> action)
    {
        int i = 0;
        foreach (var item in list) action(item, i++);
    }

    public static void Each<T>(this IEnumerable<T> list, Action<int> action)
    {
        int i = 0;
        foreach (var item in list) action(i++);
    }
}