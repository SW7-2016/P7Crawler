using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.Reviews
{
    class UserReview : Review
    {
        public double credibility;

        public UserReview(int id, string url, string category, bool verifiedPurchase) : base(id, url, category)
        {
            VerifiedPurchase = verifiedPurchase;
        }

        public bool VerifiedPurchase { get; }
    }