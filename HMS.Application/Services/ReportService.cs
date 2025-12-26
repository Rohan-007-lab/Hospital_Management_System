//using HMS.Application.Interfaces;
//using QuestPDF.Infrastructure;
//using System.Reflection.Metadata;

//namespace HMS.Application.Services;

//public class ReportService : IReportService
//{
//    private readonly IUnitOfWork _unitOfWork;

//    public ReportService(IUnitOfWork unitOfWork)
//    {
//        _unitOfWork = unitOfWork;
//        QuestPDF.Settings.License = LicenseType.Community;
//    }

//    public async Task<byte[]> GeneratePrescriptionPdfAsync(int prescriptionId)
//    {
//        var prescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.Id == prescriptionId);
//        var prescription = prescriptions.FirstOrDefault();

//        if (prescription == null)
//        {
//            throw new Exception("Prescription not found");
//        }

//        // Load navigation properties
//        var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == prescription.PatientId);
//        prescription.Patient = patients.FirstOrDefault()!;

//        var patientUsers = await _unitOfWork.Users.FindAsync(u => u.Id == prescription.Patient.UserId);
//        prescription.Patient.User = patientUsers.FirstOrDefault()!;

//        var doctors = await _unitOfWork.Doctors.FindAsync(d => d.Id == prescription.DoctorId);
//        prescription.Doctor = doctors.FirstOrDefault()!;

//        var doctorUsers = await _unitOfWork.Users.FindAsync(u => u.Id == prescription.Doctor.UserId);
//        prescription.Doctor.User = doctorUsers.FirstOrDefault()!;

//        var prescriptionMedicines = await _unitOfWork.PrescriptionMedicines.FindAsync(pm =>
//            pm.PrescriptionId == prescription.Id);
//        prescription.PrescriptionMedicines = prescriptionMedicines.ToList();

//        foreach (var pm in prescription.PrescriptionMedicines)
//        {
//            var medicine = await _unitOfWork.Medicines.GetByIdAsync(pm.MedicineId);
//            pm.Medicine = medicine!;
//        }

//        var document = Document.Create(container =>
//        {
//            container.Page(page =>
//            {
//                page.Size(PageSizes.A4);
//                page.Margin(2, Unit.Centimetre);
//                page.PageColor(Colors.White);
//                page.DefaultTextStyle(x => x.FontSize(11));

//                page.Header()
//                    .Text("PRESCRIPTION")
//                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

//                page.Content()
//                    .PaddingVertical(1, Unit.Centimetre)
//                    .Column(x =>
//                    {
//                        x.Spacing(15);

//                        // Hospital Info
//                        x.Item().Text("Hospital Management System").FontSize(16).SemiBold();
//                        x.Item().Text("123 Medical Street, City").FontSize(10);
//                        x.Item().Text("Phone: +1234567890").FontSize(10);

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Prescription Details
//                        x.Item().Row(row =>
//                        {
//                            row.RelativeItem().Column(col =>
//                            {
//                                col.Item().Text($"Prescription No: {prescription.PrescriptionNumber}").SemiBold();
//                                col.Item().Text($"Date: {prescription.PrescriptionDate:dd/MM/yyyy}");
//                            });
//                        });

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Patient Details
//                        x.Item().Text("Patient Information").SemiBold().FontSize(14);
//                        x.Item().Text($"Name: {prescription.Patient.User.FirstName} {prescription.Patient.User.LastName}");
//                        x.Item().Text($"Age: {DateTime.Now.Year - prescription.Patient.User.DateOfBirth.Year} years");
//                        x.Item().Text($"Gender: {prescription.Patient.User.Gender}");
//                        x.Item().Text($"Blood Group: {prescription.Patient.BloodGroup}");

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Doctor Details
//                        x.Item().Text("Doctor Information").SemiBold().FontSize(14);
//                        x.Item().Text($"Dr. {prescription.Doctor.User.FirstName} {prescription.Doctor.User.LastName}");
//                        x.Item().Text($"Specialization: {prescription.Doctor.Specialization}");

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Diagnosis
//                        x.Item().Text("Diagnosis").SemiBold().FontSize(14);
//                        x.Item().Text(prescription.Diagnosis);

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Medicines Table
//                        x.Item().Text("Prescribed Medicines").SemiBold().FontSize(14);

//                        x.Item().Table(table =>
//                        {
//                            table.ColumnsDefinition(columns =>
//                            {
//                                columns.RelativeColumn(3);
//                                columns.RelativeColumn(2);
//                                columns.RelativeColumn(2);
//                                columns.RelativeColumn(1);
//                                columns.RelativeColumn(3);
//                            });

//                            table.Header(header =>
//                            {
//                                header.Cell().Element(CellStyle).Text("Medicine").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Dosage").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Frequency").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Days").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Instructions").SemiBold();

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5);
//                                }
//                            });

//                            foreach (var med in prescription.PrescriptionMedicines)
//                            {
//                                table.Cell().Element(CellStyle).Text(med.Medicine.MedicineName);
//                                table.Cell().Element(CellStyle).Text(med.Dosage);
//                                table.Cell().Element(CellStyle).Text(med.Frequency);
//                                table.Cell().Element(CellStyle).Text(med.DurationDays.ToString());
//                                table.Cell().Element(CellStyle).Text(med.Instructions);

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
//                                }
//                            }
//                        });

//                        // General Instructions
//                        if (!string.IsNullOrEmpty(prescription.GeneralInstructions))
//                        {
//                            x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
//                            x.Item().Text("General Instructions").SemiBold().FontSize(14);
//                            x.Item().Text(prescription.GeneralInstructions);
//                        }

//                        // Follow Up
//                        if (!string.IsNullOrEmpty(prescription.FollowUpDate))
//                        {
//                            x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
//                            x.Item().Text($"Follow-up Date: {prescription.FollowUpDate}").SemiBold();
//                        }
//                    });

//                page.Footer()
//                    .AlignCenter()
//                    .Text(x =>
//                    {
//                        x.Span("Generated on: ");
//                        x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
//                    });
//            });
//        });

//        return document.GeneratePdf();
//    }

//    public async Task<byte[]> GenerateBillPdfAsync(int billId)
//    {
//        var bills = await _unitOfWork.Bills.FindAsync(b => b.Id == billId);
//        var bill = bills.FirstOrDefault();

//        if (bill == null)
//        {
//            throw new Exception("Bill not found");
//        }

//        // Load navigation properties
//        var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == bill.PatientId);
//        bill.Patient = patients.FirstOrDefault()!;

//        var users = await _unitOfWork.Users.FindAsync(u => u.Id == bill.Patient.UserId);
//        bill.Patient.User = users.FirstOrDefault()!;

//        var billItems = await _unitOfWork.BillItems.FindAsync(bi => bi.BillId == bill.Id);
//        bill.BillItems = billItems.ToList();

//        var document = Document.Create(container =>
//        {
//            container.Page(page =>
//            {
//                page.Size(PageSizes.A4);
//                page.Margin(2, Unit.Centimetre);
//                page.PageColor(Colors.White);
//                page.DefaultTextStyle(x => x.FontSize(11));

//                page.Header()
//                    .Text("INVOICE")
//                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

//                page.Content()
//                    .PaddingVertical(1, Unit.Centimetre)
//                    .Column(x =>
//                    {
//                        x.Spacing(15);

//                        // Hospital Info
//                        x.Item().Text("Hospital Management System").FontSize(16).SemiBold();
//                        x.Item().Text("123 Medical Street, City").FontSize(10);
//                        x.Item().Text("Phone: +1234567890 | Email: info@hospital.com").FontSize(10);

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Bill Details
//                        x.Item().Row(row =>
//                        {
//                            row.RelativeItem().Column(col =>
//                            {
//                                col.Item().Text($"Bill No: {bill.BillNumber}").SemiBold();
//                                col.Item().Text($"Date: {bill.BillDate:dd/MM/yyyy}");
//                            });

//                            row.RelativeItem().Column(col =>
//                            {
//                                col.Item().AlignRight().Text($"Payment Status: {bill.PaymentStatus}").SemiBold();
//                            });
//                        });

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Patient Details
//                        x.Item().Text("Bill To:").SemiBold().FontSize(14);
//                        x.Item().Text($"{bill.Patient.User.FirstName} {bill.Patient.User.LastName}");
//                        x.Item().Text($"Patient No: {bill.Patient.PatientNumber}");
//                        x.Item().Text($"Phone: {bill.Patient.User.PhoneNumber}");
//                        x.Item().Text($"Email: {bill.Patient.User.Email}");

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Bill Items Table
//                        x.Item().Table(table =>
//                        {
//                            table.ColumnsDefinition(columns =>
//                            {
//                                columns.ConstantColumn(50);
//                                columns.RelativeColumn(4);
//                                columns.RelativeColumn(1);
//                                columns.RelativeColumn(2);
//                                columns.RelativeColumn(2);
//                            });

//                            table.Header(header =>
//                            {
//                                header.Cell().Element(CellStyle).Text("#").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Description").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Qty").SemiBold();
//                                header.Cell().Element(CellStyle).AlignRight().Text("Unit Price").SemiBold();
//                                header.Cell().Element(CellStyle).AlignRight().Text("Total").SemiBold();

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5);
//                                }
//                            });

//                            int index = 1;
//                            foreach (var item in bill.BillItems)
//                            {
//                                table.Cell().Element(CellStyle).Text(index.ToString());
//                                table.Cell().Element(CellStyle).Text($"{item.ItemType} - {item.Description}");
//                                table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
//                                table.Cell().Element(CellStyle).AlignRight().Text($"₹{item.UnitPrice:N2}");
//                                table.Cell().Element(CellStyle).AlignRight().Text($"₹{item.TotalPrice:N2}");

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
//                                }

//                                index++;
//                            }
//                        });

//                        x.Item().PaddingTop(10);

//                        // Totals
//                        x.Item().Row(row =>
//                        {
//                            row.RelativeItem();
//                            row.ConstantItem(200).Column(col =>
//                            {
//                                col.Item().Row(r =>
//                                {
//                                    r.RelativeItem().Text("Subtotal:");
//                                    r.ConstantItem(100).AlignRight().Text($"₹{bill.SubTotal:N2}");
//                                });

//                                col.Item().Row(r =>
//                                {
//                                    r.RelativeItem().Text("Tax:");
//                                    r.ConstantItem(100).AlignRight().Text($"₹{bill.TaxAmount:N2}");
//                                });

//                                col.Item().Row(r =>
//                                {
//                                    r.RelativeItem().Text("Discount:");
//                                    r.ConstantItem(100).AlignRight().Text($"₹{bill.Discount:N2}");
//                                });

//                                col.Item().LineHorizontal(1).LineColor(Colors.Black);

//                                col.Item().Row(r =>
//                                {
//                                    r.RelativeItem().Text("Total Amount:").SemiBold().FontSize(14);
//                                    r.ConstantItem(100).AlignRight().Text($"₹{bill.TotalAmount:N2}").SemiBold().FontSize(14);
//                                });

//                                col.Item().Row(r =>
//                                {
//                                    r.RelativeItem().Text("Paid Amount:");
//                                    r.ConstantItem(100).AlignRight().Text($"₹{bill.PaidAmount:N2}");
//                                });

//                                col.Item().Row(r =>
//                                {
//                                    r.RelativeItem().Text("Balance:").SemiBold();
//                                    r.ConstantItem(100).AlignRight().Text($"₹{bill.BalanceAmount:N2}").SemiBold().FontColor(Colors.Red.Medium);
//                                });
//                            });
//                        });

//                        // Payment Details
//                        if (!string.IsNullOrEmpty(bill.PaymentMethod))
//                        {
//                            x.Item().PaddingTop(15);
//                            x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
//                            x.Item().Text("Payment Information").SemiBold().FontSize(12);
//                            x.Item().Text($"Payment Method: {bill.PaymentMethod}");
//                            if (!string.IsNullOrEmpty(bill.TransactionId))
//                            {
//                                x.Item().Text($"Transaction ID: {bill.TransactionId}");
//                            }
//                        }

//                        // Notes
//                        if (!string.IsNullOrEmpty(bill.Notes))
//                        {
//                            x.Item().PaddingTop(15);
//                            x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
//                            x.Item().Text("Notes:").SemiBold();
//                            x.Item().Text(bill.Notes);
//                        }

//                        // Thank You Message
//                        x.Item().PaddingTop(20);
//                        x.Item().AlignCenter().Text("Thank you for choosing our services!").FontSize(12).Italic();
//                    });

//                page.Footer()
//                    .AlignCenter()
//                    .Text(x =>
//                    {
//                        x.Span("Generated on: ");
//                        x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
//                    });
//            });
//        });

//        return document.GeneratePdf();
//    }

//    public async Task<byte[]> GenerateLabReportPdfAsync(int labTestId)
//    {
//        var labTests = await _unitOfWork.LabTests.FindAsync(l => l.Id == labTestId);
//        var labTest = labTests.FirstOrDefault();

//        if (labTest == null)
//        {
//            throw new Exception("Lab test not found");
//        }

//        // Load navigation properties
//        var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == labTest.PatientId);
//        labTest.Patient = patients.FirstOrDefault()!;

//        var users = await _unitOfWork.Users.FindAsync(u => u.Id == labTest.Patient.UserId);
//        labTest.Patient.User = users.FirstOrDefault()!;

//        var document = Document.Create(container =>
//        {
//            container.Page(page =>
//            {
//                page.Size(PageSizes.A4);
//                page.Margin(2, Unit.Centimetre);
//                page.PageColor(Colors.White);
//                page.DefaultTextStyle(x => x.FontSize(11));

//                page.Header()
//                    .Text("LABORATORY REPORT")
//                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

//                page.Content()
//                    .PaddingVertical(1, Unit.Centimetre)
//                    .Column(x =>
//                    {
//                        x.Spacing(15);

//                        // Hospital Info
//                        x.Item().Text("Hospital Management System").FontSize(16).SemiBold();
//                        x.Item().Text("Laboratory Department").FontSize(12);
//                        x.Item().Text("123 Medical Street, City | Phone: +1234567890").FontSize(10);

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Test Details
//                        x.Item().Row(row =>
//                        {
//                            row.RelativeItem().Column(col =>
//                            {
//                                col.Item().Text($"Test No: {labTest.TestNumber}").SemiBold();
//                                col.Item().Text($"Requested Date: {labTest.RequestedDate:dd/MM/yyyy}");
//                                if (labTest.ReportDate.HasValue)
//                                {
//                                    col.Item().Text($"Report Date: {labTest.ReportDate:dd/MM/yyyy}");
//                                }
//                            });

//                            row.RelativeItem().Column(col =>
//                            {
//                                col.Item().AlignRight().Text($"Status: {labTest.Status}").SemiBold();
//                            });
//                        });

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Patient Details
//                        x.Item().Text("Patient Information").SemiBold().FontSize(14);
//                        x.Item().Text($"Name: {labTest.Patient.User.FirstName} {labTest.Patient.User.LastName}");
//                        x.Item().Text($"Patient No: {labTest.Patient.PatientNumber}");
//                        x.Item().Text($"Age: {DateTime.Now.Year - labTest.Patient.User.DateOfBirth.Year} years");
//                        x.Item().Text($"Gender: {labTest.Patient.User.Gender}");
//                        x.Item().Text($"Blood Group: {labTest.Patient.BloodGroup}");

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Test Information
//                        x.Item().Text("Test Information").SemiBold().FontSize(14);
//                        x.Item().Text($"Test Name: {labTest.TestName}").SemiBold();
//                        x.Item().Text($"Test Type: {labTest.TestType}");

//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                        // Test Results
//                        x.Item().Text("Test Results").SemiBold().FontSize(14);

//                        if (!string.IsNullOrEmpty(labTest.Results))
//                        {
//                            x.Item().Border(1).BorderColor(Colors.Grey.Lighten1)
//                                .Padding(10)
//                                .Text(labTest.Results);
//                        }
//                        else
//                        {
//                            x.Item().Text("Results pending...").Italic().FontColor(Colors.Grey.Medium);
//                        }

//                        // Technician Notes
//                        if (!string.IsNullOrEmpty(labTest.TechnicianNotes))
//                        {
//                            x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
//                            x.Item().Text("Technician Notes:").SemiBold();
//                            x.Item().Text(labTest.TechnicianNotes);
//                        }

//                        // Signature Section
//                        x.Item().PaddingTop(30);
//                        x.Item().Row(row =>
//                        {
//                            row.RelativeItem().Column(col =>
//                            {
//                                col.Item().Text("_____________________");
//                                col.Item().Text("Lab Technician").FontSize(10);
//                            });

//                            row.RelativeItem().Column(col =>
//                            {
//                                col.Item().AlignRight().Text("_____________________");
//                                col.Item().AlignRight().Text("Verified By").FontSize(10);
//                            });
//                        });

//                        // Disclaimer
//                        x.Item().PaddingTop(20);
//                        x.Item().Border(1).BorderColor(Colors.Grey.Lighten2)
//                            .Background(Colors.Grey.Lighten3)
//                            .Padding(10)
//                            .Text("Note: This is a computer-generated report. Please consult with your doctor for proper interpretation of results.")
//                            .FontSize(9).Italic();
//                    });

//                page.Footer()
//                    .AlignCenter()
//                    .Text(x =>
//                    {
//                        x.Span("Generated on: ");
//                        x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
//                        x.Span(" | Page ");
//                        x.CurrentPageNumber();
//                    });
//            });
//        });

//        return document.GeneratePdf();
//    }

//    public async Task<byte[]> GeneratePatientReportPdfAsync(int patientId)
//    {
//        var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == patientId);
//        var patient = patients.FirstOrDefault();

//        if (patient == null)
//        {
//            throw new Exception("Patient not found");
//        }

//        // Load user
//        var users = await _unitOfWork.Users.FindAsync(u => u.Id == patient.UserId);
//        patient.User = users.FirstOrDefault()!;

//        // Load appointments
//        var appointments = await _unitOfWork.Appointments.FindAsync(a => a.PatientId == patientId);
//        var appointmentsList = appointments.OrderByDescending(a => a.AppointmentDate).Take(10).ToList();

//        // Load prescriptions
//        var prescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.PatientId == patientId);
//        var prescriptionsList = prescriptions.OrderByDescending(p => p.PrescriptionDate).Take(10).ToList();

//        // Load lab tests
//        var labTests = await _unitOfWork.LabTests.FindAsync(l => l.PatientId == patientId);
//        var labTestsList = labTests.OrderByDescending(l => l.RequestedDate).Take(10).ToList();

//        var document = Document.Create(container =>
//        {
//            container.Page(page =>
//            {
//                page.Size(PageSizes.A4);
//                page.Margin(2, Unit.Centimetre);
//                page.PageColor(Colors.White);
//                page.DefaultTextStyle(x => x.FontSize(11));

//                page.Header()
//                .Text("PATIENT MEDICAL REPORT")
//                .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

//                page.Content()
//                .PaddingVertical(1, Unit.Centimetre)
//                .Column(x =>
//                {
//                    x.Spacing(15);

//                    // Hospital Info
//                    x.Item().Text("Hospital Management System").FontSize(16).SemiBold();
//                    x.Item().Text("Comprehensive Patient Report").FontSize(12);
//                    x.Item().Text($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);

//                    x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                    // Patient Information
//                    x.Item().Text("Patient Information").SemiBold().FontSize(14).FontColor(Colors.Blue.Medium);
//                    x.Item().Row(row =>
//                    {
//                        row.RelativeItem().Column(col =>
//                        {
//                            col.Item().Text($"Name: {patient.User.FirstName} {patient.User.LastName}").SemiBold();
//                            col.Item().Text($"Patient No: {patient.PatientNumber}");
//                            col.Item().Text($"Age: {DateTime.Now.Year - patient.User.DateOfBirth.Year} years");
//                            col.Item().Text($"Gender: {patient.User.Gender}");
//                        });

//                        row.RelativeItem().Column(col =>
//                        {
//                            col.Item().Text($"Blood Group: {patient.BloodGroup}");
//                            col.Item().Text($"Phone: {patient.User.PhoneNumber}");
//                            col.Item().Text($"Email: {patient.User.Email}");
//                        });
//                    });

//                    // Medical History
//                    if (!string.IsNullOrEmpty(patient.MedicalHistory))
//                    {
//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
//                        x.Item().Text("Medical History").SemiBold().FontSize(12);
//                        x.Item().Text(patient.MedicalHistory);
//                    }

//                    // Allergies
//                    if (!string.IsNullOrEmpty(patient.Allergies))
//                    {
//                        x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
//                        x.Item().Text("Known Allergies").SemiBold().FontSize(12).FontColor(Colors.Red.Medium);
//                        x.Item().Text(patient.Allergies);
//                    }

//                    // Recent Appointments
//                    x.Item().PageBreak();
//                    x.Item().Text("Recent Appointments (Last 10)").SemiBold().FontSize(14).FontColor(Colors.Blue.Medium);

//                    if (appointmentsList.Any())
//                    {
//                        x.Item().Table(table =>
//                        {
//                            table.ColumnsDefinition(columns =>
//                            {
//                                columns.RelativeColumn(2);
//                                columns.RelativeColumn(2);
//                                columns.RelativeColumn(3);
//                                columns.RelativeColumn(2);
//                            });

//                            table.Header(header =>
//                            {
//                                header.Cell().Element(CellStyle).Text("Date").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Time").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Reason").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Status").SemiBold();

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5);
//                                }
//                            });

//                            foreach (var apt in appointmentsList)
//                            {
//                                table.Cell().Element(CellStyle).Text(apt.AppointmentDate.ToString("dd/MM/yyyy"));
//                                table.Cell().Element(CellStyle).Text(apt.AppointmentTime.ToString());
//                                table.Cell().Element(CellStyle).Text(apt.ReasonForVisit);
//                                table.Cell().Element(CellStyle).Text(apt.Status.ToString());

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
//                                }
//                            }
//                        });
//                    }
//                    else
//                    {
//                        x.Item().Text("No appointments recorded.").Italic();
//                    }

//                    // Recent Prescriptions
//                    x.Item().PaddingTop(20);
//                    x.Item().Text("Recent Prescriptions (Last 10)").SemiBold().FontSize(14).FontColor(Colors.Blue.Medium);

//                    if (prescriptionsList.Any())
//                    {
//                        x.Item().Table(table =>
//                        {
//                            table.ColumnsDefinition(columns =>
//                            {
//                                columns.RelativeColumn(2);
//                                columns.RelativeColumn(3);
//                                columns.RelativeColumn(4);
//                            });

//                            table.Header(header =>
//                            {
//                                header.Cell().Element(CellStyle).Text("Date").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Prescription No").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Diagnosis").SemiBold();

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5);
//                                }
//                            });

//                            foreach (var pres in prescriptionsList)
//                            {
//                                table.Cell().Element(CellStyle).Text(pres.PrescriptionDate.ToString("dd/MM/yyyy"));
//                                table.Cell().Element(CellStyle).Text(pres.PrescriptionNumber);
//                                table.Cell().Element(CellStyle).Text(pres.Diagnosis);

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
//                                }
//                            }
//                        });
//                    }
//                    else
//                    {
//                        x.Item().Text("No prescriptions recorded.").Italic();
//                    }




//                    // Recent Lab Tests
//                    x.Item().PaddingTop(20);
//                    x.Item().Text("Recent Lab Tests (Last 10)").SemiBold().FontSize(14).FontColor(Colors.Blue.Medium);

//                    if (labTestsList.Any())
//                    {
//                        x.Item().Table(table =>
//                        {
//                            table.ColumnsDefinition(columns =>
//                            {
//                                columns.RelativeColumn(2);
//                                columns.RelativeColumn(3);
//                                columns.RelativeColumn(2);
//                                columns.RelativeColumn(2);
//                            });

//                            table.Header(header =>
//                            {
//                                header.Cell().Element(CellStyle).Text("Date").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Test Name").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Test No").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Status").SemiBold();

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5);
//                                }
//                            });

//                            foreach (var test in labTestsList)
//                            {
//                                table.Cell().Element(CellStyle).Text(test.RequestedDate.ToString("dd/MM/yyyy"));
//                                table.Cell().Element(CellStyle).Text(test.TestName);
//                                table.Cell().Element(CellStyle).Text(test.TestNumber);
//                                table.Cell().Element(CellStyle).Text(test.Status.ToString());

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
//                                }
//                            }
//                        });
//                    }
//                    else
//                    {
//                        x.Item().Text("No lab tests recorded.").Italic();
//                    }

//                    if (labTestsList.Any())
//                    {
//                        x.Item().Table(table =>
//                        {
//                            table.ColumnsDefinition(columns =>
//                            {
//                                columns.RelativeColumn(2);
//                                columns.RelativeColumn(3);
//                                columns.RelativeColumn(2);
//                                columns.RelativeColumn(2);
//                            });

//                            table.Header(header =>
//                            {
//                                header.Cell().Element(CellStyle).Text("Date").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Test Name").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Test No").SemiBold();
//                                header.Cell().Element(CellStyle).Text("Status").SemiBold();

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5);
//                                }
//                            });

//                            foreach (var test in labTestsList)
//                            {
//                                table.Cell().Element(CellStyle).Text(test.RequestedDate.ToString("dd/MM/yyyy"));
//                                table.Cell().Element(CellStyle).Text(test.TestName);
//                                table.Cell().Element(CellStyle).Text(test.TestNumber);
//                                table.Cell().Element(CellStyle).Text(test.Status.ToString());

//                                static IContainer CellStyle(IContainer container)
//                                {
//                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
//                                }
//                            }
//                        });
//                    }
//                    else
//                    {
//                        x.Item().Text("No lab tests recorded.").Italic();
//                    }

//                    // Summary
//                    x.Item().PaddingTop(20);
//                    x.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
//                    x.Item().Text("Report Summary").SemiBold().FontSize(12);
//                    x.Item().Text($"Total Appointments: {appointmentsList.Count}");
//                    x.Item().Text($"Total Prescriptions: {prescriptionsList.Count}");
//                    x.Item().Text($"Total Lab Tests: {labTestsList.Count}");

//                    // Confidentiality Notice
//                    x.Item().PaddingTop(20);
//                    x.Item().Border(1).BorderColor(Colors.Red.Lighten2)
//                        .Background(Colors.Red.Lighten4)
//                        .Padding(10)
//                        .Text("CONFIDENTIAL: This report contains sensitive patient information and should be handled according to privacy regulations.")
//                        .FontSize(9).SemiBold().FontColor(Colors.Red.Darken1);
//                });

//                page.Footer()
//                    .AlignCenter()
//                    .Text(x =>
//                    {
//                        x.Span("Page ");
//                        x.CurrentPageNumber();
//                        x.Span(" of ");
//                        x.TotalPages();
//                        x.Span(" | Generated: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
//                    });
//            });
//        });

//        return document.GeneratePdf();
//    }
//}






