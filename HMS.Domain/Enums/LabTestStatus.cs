using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Enums;

public enum LabTestStatus
{
    Requested = 1,
    SampleCollected = 2,
    InProgress = 3,
    Completed = 4,
    ReportReady = 5
}
