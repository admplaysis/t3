using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class ViewFeedbackMap : EntityTypeConfiguration<ViewFeedback>
    {
        public ViewFeedbackMap()
        {
            ToTable("V_FEEDBACKS");
        }
    }
}