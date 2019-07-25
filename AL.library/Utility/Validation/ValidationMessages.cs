using System;

namespace AL.library.Utility.Validation
{
    public class ValidationMessages
    {
        public DateTime register { get; private set; }
        public string message { get; set; }

        public ValidationMessages()
        {
            register = DateTime.Now;
        }
    }
}
