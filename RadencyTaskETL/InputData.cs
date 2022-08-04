using System;

namespace RadencyTaskETL
{
    public class InputData
    {
        public string FirstName;
        public string LastName;
        public string City;
        public decimal Payment;
        public DateOnly Date;
        public long AccountNumber;
        public string Service;

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                return false;
            if (string.IsNullOrEmpty(City) || string.IsNullOrEmpty(Service))
                return false;
            return true;
        }

        public string GetFullName() => (FirstName + " " + LastName).Trim();
    }
}