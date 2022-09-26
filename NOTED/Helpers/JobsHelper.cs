using System.Collections.Generic;

namespace NOTED.Helpers
{
    internal class JobsHelper
    {
        public static JobRoles RoleForJob(uint jobId)
        {
            if (JobRolesMap.TryGetValue(jobId, out var role))
            {
                return role;
            }

            return JobRoles.Unknown;
        }

        public static Dictionary<uint, JobRoles> JobRolesMap = new Dictionary<uint, JobRoles>()
        {
            // tanks
            [JobIDs.GLA] = JobRoles.Tank,
            [JobIDs.MRD] = JobRoles.Tank,
            [JobIDs.PLD] = JobRoles.Tank,
            [JobIDs.WAR] = JobRoles.Tank,
            [JobIDs.DRK] = JobRoles.Tank,
            [JobIDs.GNB] = JobRoles.Tank,

            // healers
            [JobIDs.CNJ] = JobRoles.Healer,
            [JobIDs.WHM] = JobRoles.Healer,
            [JobIDs.SCH] = JobRoles.Healer,
            [JobIDs.AST] = JobRoles.Healer,
            [JobIDs.SGE] = JobRoles.Healer,

            // melee dps
            [JobIDs.PGL] = JobRoles.DPSMelee,
            [JobIDs.LNC] = JobRoles.DPSMelee,
            [JobIDs.ROG] = JobRoles.DPSMelee,
            [JobIDs.MNK] = JobRoles.DPSMelee,
            [JobIDs.DRG] = JobRoles.DPSMelee,
            [JobIDs.NIN] = JobRoles.DPSMelee,
            [JobIDs.SAM] = JobRoles.DPSMelee,
            [JobIDs.RPR] = JobRoles.DPSMelee,

            // ranged phys dps
            [JobIDs.ARC] = JobRoles.DPSRanged,
            [JobIDs.BRD] = JobRoles.DPSRanged,
            [JobIDs.MCH] = JobRoles.DPSRanged,
            [JobIDs.DNC] = JobRoles.DPSRanged,

            // ranged magic dps
            [JobIDs.THM] = JobRoles.DPSCaster,
            [JobIDs.ACN] = JobRoles.DPSCaster,
            [JobIDs.BLM] = JobRoles.DPSCaster,
            [JobIDs.SMN] = JobRoles.DPSCaster,
            [JobIDs.RDM] = JobRoles.DPSCaster,
            [JobIDs.BLU] = JobRoles.DPSCaster,

            // crafters
            [JobIDs.CRP] = JobRoles.Crafter,
            [JobIDs.BSM] = JobRoles.Crafter,
            [JobIDs.ARM] = JobRoles.Crafter,
            [JobIDs.GSM] = JobRoles.Crafter,
            [JobIDs.LTW] = JobRoles.Crafter,
            [JobIDs.WVR] = JobRoles.Crafter,
            [JobIDs.ALC] = JobRoles.Crafter,
            [JobIDs.CUL] = JobRoles.Crafter,

            // gatherers
            [JobIDs.MIN] = JobRoles.Gatherer,
            [JobIDs.BOT] = JobRoles.Gatherer,
            [JobIDs.FSH] = JobRoles.Gatherer,
        };

        public static Dictionary<JobRoles, List<uint>> JobsByRole = new Dictionary<JobRoles, List<uint>>()
        {
            // tanks
            [JobRoles.Tank] = new List<uint>() {
                JobIDs.GLA,
                JobIDs.MRD,
                JobIDs.PLD,
                JobIDs.WAR,
                JobIDs.DRK,
                JobIDs.GNB,
            },

            // healers
            [JobRoles.Healer] = new List<uint>()
            {
                JobIDs.CNJ,
                JobIDs.WHM,
                JobIDs.SCH,
                JobIDs.AST,
                JobIDs.SGE
            },

            // melee dps
            [JobRoles.DPSMelee] = new List<uint>() {
                JobIDs.PGL,
                JobIDs.LNC,
                JobIDs.ROG,
                JobIDs.MNK,
                JobIDs.DRG,
                JobIDs.NIN,
                JobIDs.SAM,
                JobIDs.RPR
            },

            // ranged phys dps
            [JobRoles.DPSRanged] = new List<uint>()
            {
                JobIDs.ARC,
                JobIDs.BRD,
                JobIDs.MCH,
                JobIDs.DNC,
            },

            // ranged magic dps
            [JobRoles.DPSCaster] = new List<uint>()
            {
                JobIDs.THM,
                JobIDs.ACN,
                JobIDs.BLM,
                JobIDs.SMN,
                JobIDs.RDM,
                JobIDs.BLU,
            },

            // crafters
            [JobRoles.Crafter] = new List<uint>()
            {
                JobIDs.CRP,
                JobIDs.BSM,
                JobIDs.ARM,
                JobIDs.GSM,
                JobIDs.LTW,
                JobIDs.WVR,
                JobIDs.ALC,
                JobIDs.CUL,
            },

            // gatherers
            [JobRoles.Gatherer] = new List<uint>()
            {
                JobIDs.MIN,
                JobIDs.BOT,
                JobIDs.FSH,
            },

            // unknown
            [JobRoles.Unknown] = new List<uint>()
        };

        public static Dictionary<JobRoles, string> RoleNames = new Dictionary<JobRoles, string>()
        {
            [JobRoles.Tank] = "Tank",
            [JobRoles.Healer] = "Healer",
            [JobRoles.DPSMelee] = "Melee",
            [JobRoles.DPSRanged] = "Ranged",
            [JobRoles.DPSCaster] = "Caster",
            [JobRoles.Crafter] = "Crafter",
            [JobRoles.Gatherer] = "Gatherer",
            [JobRoles.Unknown] = "Unknown"
        };

        public static Dictionary<uint, string> JobNames = new Dictionary<uint, string>()
        {
            [JobIDs.ACN] = "ACN",
            [JobIDs.ALC] = "ALC",
            [JobIDs.ARC] = "ARC",
            [JobIDs.ARM] = "ARM",
            [JobIDs.AST] = "AST",

            [JobIDs.BLM] = "BLM",
            [JobIDs.BLU] = "BLU",
            [JobIDs.BRD] = "BRD",
            [JobIDs.BSM] = "BSM",
            [JobIDs.BOT] = "BOT",

            [JobIDs.CNJ] = "CNJ",
            [JobIDs.CRP] = "CRP",
            [JobIDs.CUL] = "CUL",

            [JobIDs.DNC] = "DNC",
            [JobIDs.DRG] = "DRG",
            [JobIDs.DRK] = "DRK",

            [JobIDs.FSH] = "FSH",

            [JobIDs.GLA] = "GLA",
            [JobIDs.GNB] = "GNB",
            [JobIDs.GSM] = "GSM",

            [JobIDs.MRD] = "MRD",
            [JobIDs.PLD] = "PLD",
            [JobIDs.WAR] = "WAR",

            [JobIDs.LNC] = "LNC",
            [JobIDs.LTW] = "LTW",

            [JobIDs.MCH] = "MCH",
            [JobIDs.MIN] = "MIN",
            [JobIDs.MNK] = "MNK",

            [JobIDs.NIN] = "NIN",

            [JobIDs.PGL] = "PGL",

            [JobIDs.RDM] = "RDM",
            [JobIDs.RPR] = "RPR",
            [JobIDs.ROG] = "ROG",

            [JobIDs.SAM] = "SAM",
            [JobIDs.SCH] = "SCH",
            [JobIDs.SGE] = "SGE",
            [JobIDs.SMN] = "SMN",

            [JobIDs.THM] = "THM",

            [JobIDs.WVR] = "WVR",

            [JobIDs.WHM] = "WHM",
        };
    }

    public enum JobRoles
    {
        Tank = 0,
        Healer = 1,
        DPSMelee = 2,
        DPSRanged = 3,
        DPSCaster = 4,
        Crafter = 5,
        Gatherer = 6,
        Unknown
    }

    internal static class JobIDs
    {
        public const uint GLA = 1;
        public const uint MRD = 3;
        public const uint PLD = 19;
        public const uint WAR = 21;
        public const uint DRK = 32;
        public const uint GNB = 37;

        public const uint CNJ = 6;
        public const uint WHM = 24;
        public const uint SCH = 28;
        public const uint AST = 33;
        public const uint SGE = 40;

        public const uint PGL = 2;
        public const uint LNC = 4;
        public const uint ROG = 29;
        public const uint MNK = 20;
        public const uint DRG = 22;
        public const uint NIN = 30;
        public const uint SAM = 34;
        public const uint RPR = 39;

        public const uint ARC = 5;
        public const uint BRD = 23;
        public const uint MCH = 31;
        public const uint DNC = 38;

        public const uint THM = 7;
        public const uint ACN = 26;
        public const uint BLM = 25;
        public const uint SMN = 27;
        public const uint RDM = 35;
        public const uint BLU = 36;

        public const uint CRP = 8;
        public const uint BSM = 9;
        public const uint ARM = 10;
        public const uint GSM = 11;
        public const uint LTW = 12;
        public const uint WVR = 13;
        public const uint ALC = 14;
        public const uint CUL = 15;

        public const uint MIN = 16;
        public const uint BOT = 17;
        public const uint FSH = 18;
    }
}
