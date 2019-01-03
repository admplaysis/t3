using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace SGI.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            #region login
            //############ CSS ###################
            bundles.Add(new StyleBundle("~/Content/login").Include(
                        "~/Content/animate.css",
                        "~/Content/bootstrap.css",
                        "~/Content/ionicons.css",
                        "~/Content/colors.css",
                        "~/Content/app.css"));

            //############ Java Script #################
            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                        "~/Script/modernizr-custom.js",
                        "~/Script/jquery.js",
                        "~/Script/bootstrap.js",
                        "~/Script/jquery-browser.js",
                        "~/Script/colors.js",
                        "~/Script/jquery-validate.js",
                        "~/Script/additional-methods.js",
                        "~/Script/app.js"));
            #endregion login

            #region principal
            //############ CSS ###################
            bundles.Add(new StyleBundle("~/Content/principal").Include(
                        "~/Content/animate.css",
                        "~/Content/bootstrap.css",
                        "~/Content/ionicons.css",
                        "~/Content/blueimp-gallery.css",
                        "~/Content/blueimp-gallery-indicator.css",
                        "~/Content/blueimp-gallery-video.css",
                        "~/Content/bootstrap-datepicker3.css",
                        "~/Content/rickshaw.css",
                        "~/Content/select2.css",
                        "~/Content/bootstrap-clockpicker.css",
                        "~/Content/nouislider.css",
                        "~/Content/bootstrap-colorpicker.css",
                        "~/Content/summernote.css",
                        "~/Content/basic.css",
                        "~/Content/dropzone.css",
                        "~/Content/bootstrap-editable.css",
                        "~/Content/jquery-bootgrid.css",
                        "~/Content/jquery-dataTables.css",
                        "~/Content/sweetalert.css",
                        "~/Content/loaders.css",
                        "~/Content/mfb.css",
                        "~/Content/colors.css",
                        "~/Content/app.css"));

            //############ Java Script #################
            bundles.Add(new ScriptBundle("~/bundles/principal").Include(
                        "~/Script/modernizr-custom..js",
                        "~/Script/jquery.js",
                        "~/Script/bootstrap.js",
                        "~/Script/jquery-browser.js",
                        "~/Script/smartadmin/app.config.js",
                        "~/Script/colors.js",
                        "~/Script/bootstrap-filestyle.js",
                        "~/Script/jquery-flot.js",
                        "~/Script/jquery-flot-categories.js",
                        "~/Script/jquery-flot-spline.js",
                        "~/Script/jquery-flot-tooltip.js",
                        "~/Script/jquery-flot-resize.js",
                        "~/Script/jquery-flot-pie.js",
                        "~/Script/jquery-flot-time.js",
                        "~/Script/jquery-flot-orderBars.js",
                        "~/Script/jquery-jvectormap.js",
                        "~/Script/jquery-jvectormap-us-mill.js",
                        "~/Script/jquery-jvectormap-world-mill.js",
                        "~/Script/jquery-easypiechart.js",
                        "~/Script/screenfull.js",
                        "~/Script/index.js",
                        "~/Script/bootstrap-datepicker.js",
                        "~/Script/jquery-knobjs.js",
                        "~/Script/d3.js",
                        "~/Script/rickshaw.js",
                        "~/Script/jquery-validate.js",
                        "~/Script/additional-methods.js",
                        "~/Script/select2.js",
                        "~/Script/bootstrap-clockpicker.js",
                        "~/Script/nouislider.js",
                        "~/Script/bootstrap-colorpicker.js",
                        "~/Script/summernote.js",
                        "~/Script/dropzone.js",
                        "~/Script/bootstrap-editable.js",
                        "~/Script/moment-with-locales.js",
                        "~/Script/gmaps.js",
                        "~/Script/jquery-bootgrid.js",
                        "~/Script/jquery-bootgrid-fa.js",
                        "~/Script/jquery-dataTables.js",
                        "~/Script/jquery-nestable.js",
                        "~/Script/sweetalert-dev.js",
                        "~/Script/masonry-pkgd.js",
                        "~/Script/imagesloaded-pkgd.js",
                        "~/Script/loaders-css.js",
                        "~/Script/jquery-localize.js",
                        "~/Script/blueimp-helper.js",
                        "~/Script/blueimp-gallery.js",
                        "~/Script/blueimp-gallery-fullscreen.js",
                        "~/Script/blueimp-gallery-indicator.js",
                        "~/Script/blueimp-gallery-video.js",
                        "~/Script/blueimp-gallery-vimeo.js",
                        "~/Script/blueimp-gallery-youtube.js",
                        "~/Script/jquery-blueimp-gallery.js",
                        "~/Script/jquery_maskedinput.js",
                        "~/Script/jquery_maskMoney.js",
                        "~/Script/app.js"));
            #endregion principal
        }
    }
}