﻿using StswExpress;

namespace WBZ.Login
{
    class D_LoginAppAbout : D
    {
        /// System version
        public string Version { get; } = Fn.AppVersion();
        /// About app
        public string AboutApp { get; } = $@"To w pełni darmowy program biznesowy posiadający szeroki asortyment funkcjonalności. Oferuje modułowy sposób zarządzania zasobami i dystrybucją.";
        /// About creators
        public string AboutCreators { get; } = $@"Twórcami aplikacji jest dwójka studentów z Państwowej Wyższej Szkoły Zawodowej im. Hipolita Cegielskiego w Gnieźnie.";
    }
}
