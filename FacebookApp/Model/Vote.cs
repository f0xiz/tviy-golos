//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Vote
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int VotingVariantId { get; set; }
        public double Score { get; set; }
    
        public virtual Users Users { get; set; }
        public virtual VotingVariants VotingVariants { get; set; }
    }
}