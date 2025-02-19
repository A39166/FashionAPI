﻿namespace FashionAPI.Configuaration
{
    public class GlobalSettings
    {
        public static AppSettings AppSettings { get; set; }
        public static void IncludeConfig(AppSettings appSettings)
        {
            AppSettings = appSettings;
        }

        public static int MATHF_ROUND_DIGITS = 5;

        public static int OTP_COUNTDOWN_MINUTES = 2;
        public static int OTP_EXPIRED_MINUTES = 2;

        public static readonly string[] IMAGES_UPLOAD_EXTENSIONS = { ".png", ".jpeg", ".jpg" };
        public static readonly string FOLDER_EXPORT = "resources";
        public static readonly string SUB_FOLDER_PRODUCT = "products";
        public static readonly string SUB_FOLDER_AVATAR = "avatar";
        public static readonly string SUB_FOLDER_CATEGORY = "category";
    }
}
