using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VC_WATERCRAFT._GLobalz
{
    public class spnLite
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public int NumberOfBytes { get; set; }
        public int FirstByteIndex { get; set; }
        public bool HiLo { get; set; } // Determines if big or small endian

        public int PGN { get; private set; }
        public int Address { get; private set; }
        public int Priority { get; private set; }

        public spnLite(string name, int value, int numberOfBytes, int firstByteIndex, bool hiLo)
        {
            Name = name;
            Value = value;
            NumberOfBytes = numberOfBytes;
            FirstByteIndex = firstByteIndex;
            HiLo = hiLo;
        }

        // Method to set the PGN components
        public void SetPGNComponents(int priority, int pgn, int address)
        {
            if (priority < 0 || priority > 7)
                throw new ArgumentOutOfRangeException(nameof(priority), "Priority must be between 0 and 7.");
            if (pgn < 0 || pgn > 0xFFFFF)
                throw new ArgumentOutOfRangeException(nameof(pgn), "PGN must be a 18-bit value (0 - 0xFFFFF).");
            if (address < 0 || address > 0xFF)
                throw new ArgumentOutOfRangeException(nameof(address), "Address must be a valid 8-bit value (0 - 0xFF).");

            Priority = priority;
            PGN = pgn;
            Address = address;
        }

        // Method to get the full PGN in integer form (29-bit identifier)
        public int GetFullPGN()
        {
            return (Priority << 26) | (PGN << 8) | Address;
        }

        // Method to get the full PGN as a hexadecimal string
        public string GetFullPGNString()
        {
            return $"0x{GetFullPGN():X8}";
        }

        // You can also provide a method to calculate from hex input
        public void SetPGNFromHexComponents(string priorityHex, string pgnHex, string addressHex)
        {
            int priority = Convert.ToInt32(priorityHex, 16);
            int pgn = Convert.ToInt32(pgnHex, 16);
            int address = Convert.ToInt32(addressHex, 16);
            SetPGNComponents(priority, pgn, address);
        }
    }
}
