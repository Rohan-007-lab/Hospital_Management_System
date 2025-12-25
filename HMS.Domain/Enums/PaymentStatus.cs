using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Enums;

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    PartiallyPaid = 3,
    Refunded = 4,
    Cancelled = 5
}