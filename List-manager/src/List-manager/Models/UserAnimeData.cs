using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace List_manager.Models
{
    /**
     * Class for the datatype being sent back to the MAL API in order to be used to update/Add an entry to the users list.
     * Use will be a form a user can input values into before posting the request 
     */
    public class UserAnimeData
    {
        public int Episode { get; set; }
        //1/watching, 2/completed, 3/onhold, 4/dropped, 6/plantowatch
        public int Status { get; set; }

        public int Score { get; set; }

        //unknown that this point in time
        public int Storage_Type { get; set; }

        public float Storage_Value { get; set; }

        public int Times_Rewatched { get; set; }

        public int Rewatch_Value { get; set; }

        public DateTime Date_Start { get; set; }

        public DateTime Date_Finish { get; set; }

        public int Priority { get; set; }

        //1=enable, 0=disable
        public int Enable_Discussion { get; set; }

        //1=enable, 0=disable
        public int Enable_Rewatching { get; set; }

        public string Comments { get; set; }

        //seperated by commma's
        public string Tags { get; set; }

        // Might create a constructor that sets defaults for these values

    }
}
