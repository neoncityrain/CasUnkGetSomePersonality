namespace ncrGetSomePersonality;

internal static class customWV
{
    internal class WoundViewTweaks
    {
        internal WoundView thisView;
        internal Body owner;
        internal bool exerciseReplaced;
        internal bool exerciseReplacedDeath;
        internal Sprite originalWOsprite;
        internal Button.ButtonClickedEvent originalWorkoutAction;
        internal bool isDirty;
        internal Dictionary<string, Sprite> UISpriteCache = [];
        internal Dictionary<string, Color> UITextColourCache = [];
        internal float? statHue;
        internal float? backHue;
        internal float? critHue;

        internal void reviseStats()
        {
            WoundView.specimenId = owner.PlayerSelf().idTag;
            thisView.bodyStatText.text =
                owner.PlayerSelf().ownerHeight + "cm | " + owner.PlayerSelf().ownerAge +
                "y | #" + owner.PlayerSelf().idTag;
            thisView.cInfo = [owner.PlayerSelf().ownerHeight, owner.PlayerSelf().ownerAge,
                owner.PlayerSelf().idTag, owner.PlayerSelf().ownerVersion];
            WoundView.firmwareVer = "v2.0" + owner.PlayerSelf().ownerVersion;
            thisView.versionText.text = "UNI-HEALTH " + WoundView.firmwareVer;

            var self = owner.PlayerSelf();
            if (characterText != null && characterText.Count > 0 && self.codeName != null && !string.IsNullOrEmpty(self.codeName) && characterText.ContainsKey(self.codeName) &&
                characterText.TryGetValue(self.codeName, out characterFile myFile))
            {
                if (myFile.woundviewVersionText != null)
                {
                    WoundView.firmwareVer = myFile.woundviewVersionText;
                }
                if (myFile.woundviewBodystatText != null)
                {
                    thisView.bodyStatText.text = myFile.woundviewBodystatText;
                }
                if (myFile.woundviewFirmwareText != null)
                {
                    thisView.versionText.text = myFile.woundviewFirmwareText;
                }
            }

            thisView.speciesText.text = owner.PlayerSelf().displayName.ToUpper() ?? thisView.speciesText.text.ToUpper();
        }

        #region Colours
        public static void RecolourUiHue(Texture2D texture, float hue)
        {
            var colors = texture.GetPixels32();

            for (var i = 0; i < colors.Length; i++)
            {
                Color.RGBToHSV(colors[i], out float h, out float s, out float v);
                if (h != hue && s != 0f && v != 0f && v != 1f)
                {
                    var a = colors[i].a;
                    h -= 0.417f;
                    if (h < 0f) { h += 1f; }
                    var newhue = hue + h;
                    if (newhue > 1f) { newhue -= 1f; }
                    colors[i] = Color.HSVToRGB(newhue, s, v);
                    colors[i].a = a;
                }
            }

            try
            {
                texture.SetPixels32(colors);
                texture.Apply();
            }
            catch (Exception ex)
            {
                Log.LogDebug("Error setting texture to sprite: " + ex);
            }
        }

        internal void recolourMeBaby()
        {
            var trans = thisView.transform;
            if (trans == null)
            {
                return;
            }

            #region General Text
            if (!UITextColourCache.ContainsKey("critText"))
            {
                UITextColourCache.Add("critText", thisView.critTextColor);
            }
            Color txt = thisView.critTextColor;
            Color.RGBToHSV(txt, out float _, out float s, out float v);
            if (critHue != null)
            {
                thisView.critTextColor = Color.HSVToRGB((float)critHue, s, v);
            }

            if (!UITextColourCache.ContainsKey("normText"))
            {
                UITextColourCache.Add("normText", thisView.normalTextColor);
            }
            txt = thisView.normalTextColor;
            Color.RGBToHSV(txt, out float _, out s, out v);
            if (statHue != null)
            {
                thisView.normalTextColor = Color.HSVToRGB((float)statHue, s, v);
            }
            #endregion
            #region Background
            if (backHue != null)
            {
                if (!UISpriteCache.ContainsKey("ComputerBack"))
                {
                    UISpriteCache.Add("ComputerBack", trans.Find("ComputerBack").GetComponent<Image>().sprite);
                }
                RecolourUiHue(trans.Find("ComputerBack").GetComponent<Image>()
                    .sprite.texture, (float)backHue);
            }
            #endregion
            #region Exp Title
            try
            {
                foreach (TextMeshProUGUI text in trans.Find(
                    "ExperimentTitle").GetComponentsInChildren<TextMeshProUGUI>())
                {
                    text?.color = thisView.normalTextColor;
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Title colour tweaks: " + ex);
            }
            #endregion
            #region Mode Toggle
            try
            {
                var mode = trans.Find("ModeToggle");
                if (mode != null && statHue != null)
                {
                    if (!UITextColourCache.ContainsKey("ModeToggleHighlight"))
                    {
                        UITextColourCache.Add("ModeToggleHighlight", mode.transform.GetComponent<Button>().colors.highlightedColor);
                    }
                    var col = mode.transform.GetComponent<Button>().colors;
                    col.normalColor = thisView.normalTextColor;
                    v += 0.2f; s -= 0.1f;
                    if (v > 1f) { v = 1f; } if (s < 0f) { s = 0f; }
                    col.highlightedColor = Color.HSVToRGB((float)statHue, s, v);
                    mode.transform.GetComponent<Button>().colors = col;
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Mode Toggle colour tweaks: " + ex);
            }
            #endregion
            #region Limbview
            try
            {
                var limbv = trans.Find("LimbView");
                if (limbv != null && statHue != null)
                {
                    var ltxt = limbv.GetComponentsInChildren<TextMeshProUGUI>();
                    if (ltxt != null)
                    {
                        foreach (TextMeshProUGUI text in ltxt)
                        {
                            text?.color = thisView.normalTextColor;
                        }
                    }

                    foreach (Image image in limbv.GetComponentsInChildren<Image>())
                    {
                        if (image != null)
                        {
                            if (!UISpriteCache.ContainsKey("limbvi" + image.name))
                            {
                                UISpriteCache.Add("limbvi" + image.name, image.sprite);
                            }
                            SetHue(image.sprite.texture, (float)statHue);
                        }
                    }
                    foreach (Sprite sprite in limbv.GetComponentsInChildren<Sprite>())
                    {
                        if (sprite != null)
                        {
                            if (!UISpriteCache.ContainsKey("limbvs" + sprite.name))
                            {
                                UISpriteCache.Add("limbvs" + sprite.name, sprite);
                            }
                            SetHue(sprite.texture, (float)statHue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Limbview colour tweaks: " + ex);
            }
            #endregion
            #region Skill Back
            try
            {
                var skillb = trans.Find("SkillsBack");
                if (skillb != null)
                {
                    if (statHue != null)
                    {
                        foreach (Image image in skillb.GetComponentsInChildren<Image>())
                        {
                            if (image != null && image.sprite != null &&
                                image.sprite.texture != null)
                            {
                                if (!UISpriteCache.ContainsKey("skillsbi" + image.name))
                                {
                                    UISpriteCache.Add("skillsbi" + image.name, image.sprite);
                                }
                                SetHue(image.sprite.texture, (float)statHue);
                            }
                        }

                        foreach (TextMeshProUGUI text in
                            skillb.GetComponentsInChildren<TextMeshProUGUI>())
                        {
                            text?.color = thisView.normalTextColor;
                        }

                        foreach (Sprite sprite in skillb.GetComponentsInChildren<Sprite>())
                        {
                            if (sprite != null && sprite.texture != null)
                            {
                                if (!UISpriteCache.ContainsKey("skillsbs" + sprite.name))
                                {
                                    UISpriteCache.Add("skillsbs" + sprite.name, sprite);
                                }
                                SetHue(sprite.texture, (float)statHue);
                            }
                        }
                    }


                    if (backHue != null)
                    {
                        var back = skillb.GetComponent<Image>().sprite;
                        if (!UISpriteCache.ContainsKey("skillsbg" + back.name))
                        {
                            UISpriteCache.Add("skillsbg" + back.name, back);
                        }
                        SetHue(back.texture, (float)backHue);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Skill colour tweaks: " + ex);
            }
            #endregion
            #region Time Back
            try
            {
                var timeb = trans.Find("TimeBack");
                if (timeb != null)
                {
                    if (statHue != null)
                    {
                        foreach (Image image in timeb.GetComponentsInChildren<Image>())
                        {
                            if (!UISpriteCache.ContainsKey("timebi" + image.name))
                            {
                                UISpriteCache.Add("timebi" + image.name, image.sprite);
                            }
                            SetHue(image.sprite.texture, (float)statHue);
                        }
                        foreach (TextMeshProUGUI text in
                            timeb.GetComponentsInChildren<TextMeshProUGUI>())
                        {
                            text.color = thisView.normalTextColor;
                        }
                        foreach (Sprite sprite in timeb.GetComponentsInChildren<Sprite>())
                        {
                            if (!UISpriteCache.ContainsKey("timebs" + sprite.name))
                            {
                                UISpriteCache.Add("timebs" + sprite.name, sprite);
                            }
                            SetHue(sprite.texture, (float)statHue);
                        }
                    }

                    if (backHue != null)
                    {
                        if (!UISpriteCache.ContainsKey("timeback"))
                        {
                            UISpriteCache.Add("timeb", timeb.GetComponent<Image>().sprite);
                        }
                        SetHue(timeb.GetComponent<Image>().sprite.texture, (float)backHue);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Time colour tweaks: " + ex);
            }
            #endregion
            #region Crit Panel
            try
            {
                var pcrit = thisView.criticalPanel;
                if (pcrit != null && critHue != null)
                {
                    var im = pcrit.GetComponent<Image>().sprite;
                    if (!UISpriteCache.ContainsKey("crit" + im.name))
                    {
                        UISpriteCache.Add("crit" + im.name, im);
                    }
                    SetHue(im.texture, (float)critHue);
                    pcrit.GetComponentInChildren<TextMeshProUGUI>()?.color =
                        thisView.critTextColor;
                    pcrit.GetComponent<TextMeshProUGUI>()?.color =
                        thisView.critTextColor;

                    foreach (Image img in
                        pcrit.GetComponentsInChildren<Image>())
                    {
                        if (img != null && img.sprite != null &&
                            img.sprite.texture != null)
                        {
                            if (!UISpriteCache.ContainsKey("criti" + img.name))
                            {
                                UISpriteCache.Add("criti" + img.name, img.sprite);
                            }
                            SetHue(img.sprite.texture, (float)critHue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Crit colour tweaks: " + ex);
            }
            #endregion
            #region Stat Menu
            try
            {
                var statm = thisView.mainStatsTrans;
                if (statm != null && statHue != null)
                {
                    var spr = statm.GetComponent<Image>().sprite;
                    if (!UISpriteCache.ContainsKey("statm" + spr.name))
                    {
                        UISpriteCache.Add("statm" + spr.name, spr);
                    }
                    RecolourUiHue(spr.texture, (float)statHue);
                    statm.Find("ECG").GetComponent<RawImage>().color =
                        thisView.normalTextColor;

                    statm.Find("VITALS").GetComponent<TextMeshProUGUI>().color =
                        thisView.normalTextColor;

                    var im = statm.GetComponentsInChildren<Image>();
                    foreach (Image image in im)
                    {
                        if (image != null && image.sprite != null &&
                            image.sprite.texture != null)
                        {
                            if (!UISpriteCache.ContainsKey("statmi" + image.name))
                            {
                                UISpriteCache.Add("statmi" + image.name, image.sprite);
                            }
                            SetHue(image.sprite.texture, (float)statHue);
                        }
                    }

                    foreach (Sprite sprite in statm.GetComponentsInChildren<Sprite>())
                    {
                        if (sprite != null && sprite.texture != null)
                        {
                            if (!UISpriteCache.ContainsKey("statms" + sprite.name))
                            {
                                UISpriteCache.Add("statms" + sprite.name, sprite);
                            }
                            SetHue(sprite.texture, (float)statHue);
                        }
                    }

                    foreach (Sprite mood in thisView.moodIcons)
                    {
                        SetHue(mood.texture, (float)statHue);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Statmenu colour tweaks: " + ex);
            }
            #endregion
        }

        internal void returnToDefault()
        {
            var trans = thisView.transform;
            if (trans == null || UISpriteCache == null ||
                UISpriteCache.Count <= 0)
            {
                return;
            }

            #region General Text
            if (UITextColourCache.ContainsKey("critText") &&
                UITextColourCache.TryGetValue("critText", out var crit))
            {
                thisView.critTextColor = crit;
            }

            if (!UITextColourCache.ContainsKey("normText") &&
                UITextColourCache.TryGetValue("normText", out var norm))
            {
                thisView.normalTextColor = norm;
            }
            #endregion
            #region Background
            if (UISpriteCache.ContainsKey("ComputerBack") &&
                UISpriteCache.TryGetValue("ComputerBack", out var bg))
            {
                trans.Find("ComputerBack").GetComponent<Image>().sprite = bg;
            }
            #endregion
            #region Exp Title
            try
            {
                foreach (TextMeshProUGUI text in trans.Find(
                    "ExperimentTitle").GetComponentsInChildren<TextMeshProUGUI>())
                {
                    text?.color = thisView.normalTextColor;
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Title colour tweaks: " + ex);
            }
            #endregion
            #region Mode Toggle
            try
            {
                var mode = trans.Find("ModeToggle");
                if (mode != null && statHue != null &&
                    UITextColourCache.ContainsKey("ModeToggleHighlight") &&
                    UITextColourCache.TryGetValue("ModeToggleHighlight", out var hl))
                    {
                        var col = mode.transform.GetComponent<Button>().colors;
                        col.normalColor = thisView.normalTextColor;
                        col.highlightedColor = hl;
                        mode.transform.GetComponent<Button>().colors = col;
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Mode Toggle colour tweaks: " + ex);
            }
            #endregion
            #region Limbview
            try
            {
                var limbv = trans.Find("LimbView");
                if (limbv != null && statHue != null)
                {
                    foreach (TextMeshProUGUI text in limbv.GetComponentsInChildren<TextMeshProUGUI>())
                    {
                        text?.color = thisView.normalTextColor;
                    }

                    foreach (Image image in limbv.GetComponentsInChildren<Image>())
                    {
                        if (image != null &&
                            UISpriteCache.ContainsKey("limbvi" + image.name) &&
                            UISpriteCache.TryGetValue("limbvi" + image.name, out var limb))
                        {
                            image.sprite = limb;
                        }
                    }

                    var sprites = limbv.GetComponentsInChildren<Sprite>();
                    for (int i = 0; i < sprites.Count(); i++)
                    {
                        if (sprites != null &&
                            UISpriteCache.ContainsKey("limbvs" + sprites[i].name) && UISpriteCache.TryGetValue("limbvs" + sprites[i].name, out var limb))
                            {
                                sprites[i] = limb;
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Limbview colour tweaks: " + ex);
            }
            #endregion
            #region Skill Back
            try
            {
                var skillb = trans.Find("SkillsBack");
                if (skillb != null)
                {
                    if (statHue != null)
                    {
                        foreach (Image image in skillb.GetComponentsInChildren<Image>())
                        {
                            if (image != null && image.sprite != null &&
                                image.sprite.texture != null)
                            {
                                if (UISpriteCache.ContainsKey("skillsbi" + image.name) && UISpriteCache.TryGetValue("skillsbi" + image.name, out var im))
                                {
                                    image.sprite = im;
                                }
                            }
                        }

                        foreach (TextMeshProUGUI text in
                            skillb.GetComponentsInChildren<TextMeshProUGUI>())
                        {
                            text?.color = thisView.normalTextColor;
                        }

                        var sprites = skillb.GetComponentsInChildren<Sprite>();
                        for (int i = 0; i < sprites.Count(); i++)
                        {
                            if (sprites[i] != null && sprites[i].texture != null)
                            {
                                if (UISpriteCache.ContainsKey("skillsbs" + sprites[i].name) && UISpriteCache.TryGetValue("skillsbs" + sprites[i].name, out var spr))
                                {
                                    sprites[i] = spr;
                                }
                            }
                        }
                    }

                    if (backHue != null)
                    {
                        var back = skillb.GetComponent<Image>().sprite;
                        if (UISpriteCache.ContainsKey("skillsbg" + back.name) &&
                            UISpriteCache.TryGetValue("skillsbg" + back.name, out var skillbg))
                        {
                            back = skillbg;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Skill colour tweaks: " + ex);
            }
            #endregion
            #region Time Back
            try
            {
                var timeb = trans.Find("TimeBack");
                if (timeb != null)
                {
                    if (statHue != null)
                    {
                        foreach (Image image in timeb.GetComponentsInChildren<Image>())
                        {
                            if (UISpriteCache.ContainsKey("timebi" + image.name) &&
                                UISpriteCache.TryGetValue("timebi" + image.name, out var timg))
                            {
                                image.sprite = timg;
                            }
                        }
                        foreach (TextMeshProUGUI text in
                            timeb.GetComponentsInChildren<TextMeshProUGUI>())
                        {
                            text.color = thisView.normalTextColor;
                        }

                        var sprites = timeb.GetComponentsInChildren<Sprite>();
                        for (int i = 0; i < sprites.Count(); i++)
                        {
                            if (UISpriteCache.ContainsKey("timeb" + sprites[i].name) && UISpriteCache.TryGetValue("timeb" + sprites[i].name, out var spr))
                            {
                                sprites[i] = spr;
                            }
                        }
                    }

                    if (backHue != null)
                    {
                        if (UISpriteCache.ContainsKey("timeback") &&
                            UISpriteCache.TryGetValue("timeback", out var spr))
                        {
                            timeb.GetComponent<Image>().sprite = spr;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Time colour tweaks: " + ex);
            }
            #endregion
            #region Crit Panel
            try
            {
                var pcrit = thisView.criticalPanel;
                if (pcrit != null && critHue != null)
                {
                    var im = pcrit.GetComponent<Image>().sprite;
                    if (UISpriteCache.TryGetValue("crit" + im.name, out var sprite))
                    {
                        im = sprite;
                    }
                    pcrit.GetComponentInChildren<TextMeshProUGUI>()?.color =
                        thisView.critTextColor;
                    pcrit.GetComponent<TextMeshProUGUI>()?.color =
                        thisView.critTextColor;

                    foreach (Image img in
                        pcrit.GetComponentsInChildren<Image>())
                    {
                        if (img != null && img.sprite != null &&
                            img.sprite.texture != null)
                        {
                            if (UISpriteCache.ContainsKey("criti" + img.name) &&
                                UISpriteCache.TryGetValue("criti" + img.name, out var newim))
                            {
                                img.sprite = newim;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Crit colour tweaks: " + ex);
            }
            #endregion
            #region Stat Menu
            try
            {
                var statm = thisView.mainStatsTrans;
                if (statm != null && statHue != null)
                {
                    var spr = statm.GetComponent<Image>().sprite;
                    if (UISpriteCache.ContainsKey("statm" + spr.name) &&
                        UISpriteCache.TryGetValue("statm" + spr.name, out var sprite))
                    {
                        spr = sprite;
                    }
                    statm.Find("ECG").GetComponent<RawImage>().color =
                        thisView.normalTextColor;

                    statm.Find("VITALS").GetComponent<TextMeshProUGUI>().color =
                        thisView.normalTextColor;

                    var im = statm.GetComponentsInChildren<Image>();
                    foreach (Image image in im)
                    {
                        if (image != null && image.sprite != null &&
                            image.sprite.texture != null)
                        {
                            if (UISpriteCache.ContainsKey("statmi" + image.name) &&
                               UISpriteCache.TryGetValue("statmi" + image.name, out var statmi))
                            {
                                image.sprite = statmi;
                            }
                        }
                    }

                    var sprites = statm.GetComponentsInChildren<Sprite>();
                    for (int i = 0; i < sprites.Count(); i++)
                    {
                        if (sprites[i] != null && sprites[i].texture != null)
                        {
                            if (UISpriteCache.ContainsKey("statms" + sprites[i].name) && UISpriteCache.TryGetValue("statms" + sprites[i].name, out var ms))
                            {
                                sprites[i] = ms;
                            }
                        }
                    }

                    Color.RGBToHSV(thisView.normalTextColor, out float hue, out float _, out float _);
                    foreach (Sprite mood in thisView.moodIcons)
                    {
                        SetHue(mood.texture, hue);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Issue with Statmenu colour tweaks: " + ex);
            }
            #endregion

            UISpriteCache.Clear();
        }
        #endregion

        #region Self-Destructive
        internal void setNormalExercise()
        {
            if (exerciseReplaced || exerciseReplacedDeath)
            {
                exerciseReplaced = false;
                exerciseReplacedDeath = false;
                Transform workoutButton = thisView.transform.Find("WorkoutButton");

                if (workoutButton != null)
                {
                    Image img = workoutButton.GetComponent<Image>();
                    if (img != null)
                    {
                        Vector2 delta = img.rectTransform.sizeDelta;
                        img.sprite = originalWOsprite;
                        originalWOsprite = null;
                        img.rectTransform.sizeDelta = delta;
                    }
                    UITooltip tooltip = workoutButton.GetComponent<UITooltip>();
                    if (tooltip != null)
                    {
                        tooltip.tipName = Locale.GetOther("workout");
                        tooltip.tipDesc = Locale.GetOther("workoutdsc");
                    }
                    Button butt = workoutButton.GetComponent<Button>();
                    if (butt != null)
                    {
                        butt.onClick.RemoveAllListeners();
                        butt.onClick = originalWorkoutAction;
                        originalWorkoutAction = null;
                    }
                }
            }
        }

        internal void setCasualHarmer()
        {
            if (!exerciseReplaced || exerciseReplacedDeath)
            {
                exerciseReplaced = true;
                exerciseReplacedDeath = false;
                Transform workoutButton = thisView.transform.Find("WorkoutButton");
                if (workoutButton != null)
                {
                    Image img = workoutButton.GetComponent<Image>();
                    if (img != null)
                    {
                        Vector2 delta = img.rectTransform.sizeDelta;
                        string shSprite = Path.Combine(Path.Combine(
                        gspLocation, "Assets"), (
                        "woundView" + Path.DirectorySeparatorChar +
                        "harmer.png"));
                        if (File.Exists(shSprite))
                        {
                            Sprite harmer = LoadCustomSprite(shSprite);
                            originalWOsprite = img.sprite;
                            img.sprite = harmer;
                            img.rectTransform.sizeDelta = delta;
                        }
                    }
                    UITooltip tooltip = workoutButton.GetComponent<UITooltip>();
                    if (tooltip != null)
                    {
                        tooltip.skipLocale = true;
                        tooltip.tipName = GetBaseGSP("workoutselfharm");
                        tooltip.tipDesc = GetBaseGSP("workoutselfharmdsc");
                    }
                    Button butt = workoutButton.GetComponent<Button>();
                    if (butt != null)
                    {
                        originalWorkoutAction = butt.onClick;
                        butt.onClick.RemoveAllListeners();
                        butt.onClick.AddListener(doCasualHarm);
                    }
                }
            }
        }

        internal void setCasualKiller()
        {
            if (!exerciseReplacedDeath)
            {
                setCasualHarmer(); // to be sure variables are actually initiated properly

                exerciseReplacedDeath = true;
                exerciseReplaced = false;
                Transform workoutButton = thisView.transform.Find("WorkoutButton");
                if (workoutButton != null)
                {
                    UITooltip tooltip = workoutButton.GetComponent<UITooltip>();
                    if (tooltip != null)
                    {
                        tooltip.skipLocale = true;
                        tooltip.tipName = GetBaseGSP("workoutkill");
                        tooltip.tipDesc = GetBaseGSP("workoutkilldsc");
                    }
                    Image img = workoutButton.GetComponent<Image>();
                    if (img != null)
                    {
                        Vector2 delta = img.rectTransform.sizeDelta;
                        string shSprite = Path.Combine(Path.Combine(
                        gspLocation, "Assets"), (
                        "woundView" + Path.DirectorySeparatorChar +
                        "harmer-kill.png"));
                        if (File.Exists(shSprite))
                        {
                            Sprite harmer = LoadCustomSprite(shSprite);
                            img.sprite = harmer;
                            img.rectTransform.sizeDelta = delta;
                        }
                    }
                }
            }
        }

        public void doCasualHarm()
        {
            PlayerCamera.main.ToggleWoundView();
            if (Body.censorMood)
            {
                owner.harmer.AttemptHarm();
            }
            else
            {
                owner.harmer.harming = true;
                owner.harmer.StartCoroutine(CasualSelfHarm());
            }
        }

        internal IEnumerator CasualSelfHarm()
        {
            bool suicide = exerciseReplacedDeath;
            if ((bool)MinigameBase.main.currentMinigame)
            {
                MinigameBase.main.EndMinigame();
            }
            SelfHarmMinigame harm = new(!suicide ? 1 : Random.Range(2, 5))
            {
                triesLeft = !suicide ? Random.Range(1, 5) : Random.Range(3, 6)
            };
            MinigameBase.main.StartMinigame(harm, null);
            yield return new WaitUntil(() => !MinigameBase.main.currentMinigame);

            Limb limb = null;
            if (!suicide)
            {
                while (limb == null || limb.dismembered)
                {
                    int rand = Random.Range(3, owner.limbs.Length);
                    if (rand != 11 && rand != 14 && rand != 8 && rand != 5)
                    {
                        // aka, if the number IS NOT a hand or foot.
                        // the number also excludes the chest, hips, and head.
                        limb = owner.limbs[rand];
                    }
                }
                limb.muscleHealth -= Random.Range(10f, 15f);
                limb.skinHealth -= Random.Range(25f, 30f);
                limb.bleedAmount += Random.Range(8f, 12f);
                limb.pain += Random.Range(0f, 5f);
                owner.adrenaline += Random.Range(5f, 15f);

                owner.rb.AddForce(Random.insideUnitCircle * 500f, ForceMode2D.Impulse);
                thisView.AddImageToLimb(
                    limb, Resources.Load<Sprite>("Special/selfHarmWound"),
                    flip: false, (Limb l) => l.skinHealth >= 95f);

                owner.happiness += 35f;
                if (owner.happiness > -30)
                {
                    owner.talker.Talk(GetChar("selfharm-regulated"), force: true);
                }
                else
                {
                    owner.talker.Talk(Locale.GetCharacter("selfharm"), force: true);
                }
                owner.talker.PromptTraderResponse("selfharm");
                Sound.Play("selfharm", owner.transform.position);
                Sound.Play("harmSting",
                    Vector2.zero, twoDimensional: true, pitchShift: false, null, 0.7f);
                owner.harmer.cooldownTime = 180f;
                owner.harmer.harming = false;
            }
            else
            {
                limb = owner.limbs[1]; // the chest / throat

                limb.muscleHealth -= 30f;
                limb.skinHealth -= 70f;
                limb.bleedAmount += Random.Range(45f, 60f);
                limb.pain += Random.Range(25f, 35f);
                owner.adrenaline += Random.Range(50f, 60f);
                owner.rb.AddForce(Random.insideUnitCircle * 700f, ForceMode2D.Impulse);

                owner.happiness += 90f;
                owner.talker.Talk(Locale.GetCharacter("suicide"), null, force: true);
                owner.talker.PromptTraderResponse("selfharm");

                Sound.Play("suicide", owner.transform.position);
                MusicManager.main.PlaySong(Resources.Load<AudioClip>(
                    "Sounds/music/whathaveidone"));
                owner.harmer.cooldownTime = 500f;
                Sound.Play("harmSting", Vector2.zero,
                    twoDimensional: true, pitchShift: false, null, 1f, 0.7f);
                thisView.AddImageToLimb(limb, Resources.Load<Sprite>(
                    "Special/suicideWound"), flip: false, (Limb l) => l.skinHealth >= 95f);
            }
            
            

            GameObject clawObj = UnityEngine.Object.Instantiate(
                Resources.Load<GameObject>("ClawAnimSlow"));
            clawObj.transform.position = owner.limbs[1].transform.position +
                Vector3.left * 2f;
            clawObj.transform.SetParent(owner.harmer.transform);
            UnityEngine.Object.Destroy(clawObj, 5f);
        }
        #endregion

        internal WoundViewTweaks()
        {

        }
    }
    internal static readonly ConditionalWeakTable<WoundView, WoundViewTweaks> WoundViewCWT = new();
    internal static WoundViewTweaks CustomCall(this WoundView woundview) => WoundViewCWT.GetValue(woundview, _ => new());
}