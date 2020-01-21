using System;
using System.Linq;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Enumeration used to indicate the purpose of the authentication and/or the attributes requested
    /// </summary>
    [Flags]
    public enum ServiceIds : ushort
    {
        /// <summary>
        /// Nothing
        /// </summary>
        None = 0,

        /// <summary>
        /// The Consumer Transient Id
        /// </summary>
        ConsumerTransientId = 0,

        /// <summary>
        /// The Consumer BIN
        /// </summary>
        ConsumerBin = 16384,

        /// <summary>
        /// The Consumer Name
        /// </summary>
        Name = 4096,

        /// <summary>
        /// The Consumer Address
        /// </summary>
        Address = 1024,
        
        /// <summary>
        /// The Consumer's date of birth
        /// </summary>
        DateOfBirth = 64 + 128 + 256,

        /// <summary>
        /// The value specifying if the Consumer is 18 or older
        /// </summary>
        IsEighteenOrOlder = 64,

        /// <summary>
        /// Provide Gender attribute
        /// </summary>
        Gender = 16,

        /// <summary>
        /// Provide BSN attribute
        /// </summary>
        BSN = 1,

        /// <summary>
        /// Provides the Email attribute
        /// </summary>
        Email = 2,

        /// <summary>
        /// Provides the Telephone attribute
        /// </summary>
        Telephone = 4, 

        /// <summary>
        /// Provides the DocumentId to be signed
        /// </summary>
        Sign = 8
    }
}