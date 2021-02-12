using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaRepository
{
    public enum Status
    {
        Active = 1,
        Inactive = 2,
        Used_Cheques = 3,
        UnUsed_Cheques = 4,
        FI_Saved = 5,
        FI_Partial_Saved = 6,
        Tele_Verification = 7,
        Field_Verification = 8,
        Document_Verification = 9,
        Loan_Approved = 10,
        Loan_Accepted = 11,
        Loan_Rejected = 12,
        Disbursed = 13,
        EMI_Closed = 14,
        Pre_Closed = 15,
        Full_and_Final_Settlement = 16,
        Cancelled = 17
    }
}
