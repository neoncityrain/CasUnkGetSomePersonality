namespace ncrGetSomePersonality;

internal static class playerPersona
{
    internal class PlayerKind
    {
        #region Variables
        internal string personalityType;
        internal string desc;
        internal Dictionary<string, string> personalitySubtypes = [];
        internal string personalName;
        internal int idTag;
        internal int ownerHeight;
        internal int ownerAge;
        internal int ownerVersion;
        internal List<string> bodyTraits = [];
        internal string codeName;
        internal string displayName;
        internal Body owner;
        internal float? colorblindness;
        internal string playerTitle;
        internal string lastDialogueKey = "";
        internal bool shouldUseGenStrings = false;
        internal bool shouldUseVanStrings = false;
        internal bool npc = false;
        internal Color thoughtColour;
        internal float rightEyeHealed;


        internal Dictionary<string, Color> playerColours = [];
        internal Dictionary<string, Sprite> personalSprites = [];
        internal FacialExpression myFace;
        internal WoundView myWV;
        #endregion

        internal void reviseStats(Body body)
        {
            #region setInit
            owner = thisPlayer(body);
            myFace = owner.limbs[0].GetComponent<FacialExpression>();

            if (myWV == null || myWV.CustomCall().owner == owner)
            {
                myWV = WoundView.view;
            }

            if (myWV.CustomCall().owner == null || WoundView.view == myWV)
            {
                myWV.CustomCall().thisView = myWV;
                myWV.CustomCall().owner = owner;
            }

            ownerHeight = myWV.cInfo[0];
            ownerAge = myWV.cInfo[1];
            idTag = myWV.cInfo[2];
            ownerVersion = myWV.cInfo[3];
            var mult = 1f - (owner.heightMultiplier / ownerHeight);
            if (mult < 0f) { mult = -mult; }
            owner.heightMultiplier = mult;
            #endregion

            #region ReloadFromJson
            characterFile myFile = new();
            if (characterText != null && characterText.Count > 0 && codeName != null &&
                !string.IsNullOrEmpty(codeName) && characterText.ContainsKey(codeName))
            {
                try
                {
                    characterText.TryGetValue(codeName, out myFile);
                    reviseMyJsonBoy(myFile);
                }
                catch (Exception ex)
                {
                    Log.LogDebug("Error loading character from json file: " + ex);
                }
            }
            #endregion

            if (string.IsNullOrEmpty(personalityType))
            {
                randomizePersona();
            }
            verifyTitle();
            myWV.CustomCall().reviseStats(myFile);
            reviseColours();

            #region Re-check Disabilities
            if (!bodyTraits.Contains("amputee"))
            {
                foreach (Limb limb in owner.limbs)
                {
                    if (limb.dismembered)
                    {
                        addTrait("amputee");
                    }
                }
            }
            if (owner.eyeGone && !bodyTraits.Contains("eyetornoutleft"))
            {
                addTrait("eyetornoutleft");
            }
            if (owner.bothEyesGone && !bodyTraits.Contains("eyestornout"))
            {
                addTrait("eyestornout");
            }
            if (owner.disfigured && !bodyTraits.Contains("missingjaw"))
            {
                addTrait("missingjaw");
            }
            if (colorblindness > 0 && !bodyTraits.Contains("colorblind"))
            {
                addTrait("colorblind");
            }

            if ((colorblindness <= 0 || colorblindness == null) &&
                bodyTraits.Contains("colorblind"))
            {
                colorblindness = 1f;
            }
            #endregion
            // setPlayerTalkSound();
        }

        internal void reviseMyJsonBoy(characterFile myFile)
        {
            if (myFile == null) { return; }
            if (myFile.age > -1) { ownerAge = myFile.age; }
            if (myFile.heightCm > 0) { ownerHeight = myFile.heightCm; }
            if (myFile.id > -1) { idTag = myFile.id; }
            if (myFile.versionNumber > -1) { ownerVersion = myFile.versionNumber; }

            if (myFile.personality != null) { personalityType = myFile.personality; }
            if (myFile.name != null) { personalName = myFile.name; }
            if (myFile.displayName != null) { displayName = myFile.displayName; }
            if (myFile.bodyTraits != null && myFile.bodyTraits.Length > 0)
            {
                foreach (string trait in myFile.bodyTraits)
                {
                    addTrait(trait);
                }
            }
            if (myFile.colours != null && myFile.colours.Count > 0)
            {
                foreach (string colName in myFile.colours.Keys)
                {
                    if (playerColours.Keys.Contains(colName))
                    {
                        playerColours.Remove(colName);
                    }
                    if (myFile.colours.TryGetValue(colName, out var inttocol))
                    {
                        var col = new Color(
                            ((float)inttocol[0] / 2.55f) * 0.01f,
                            ((float)inttocol[1] / 2.55f) * 0.01f,
                            ((float)inttocol[2] / 2.55f) * 0.01f);
                        playerColours.Add(colName, col);
                    }
                }
            }
            if (myFile.description != null) { desc = myFile.description; }
        }

        internal void randomizePersona()
        {
            Random.State state = Random.state;
            Random.InitState(idTag);
            try
            {
                if (string.IsNullOrEmpty(personalityType) &&
                    personaText != null && personaText.Keys != null)
                {
                    personalityType = personaText.Keys.ElementAt(Random.Range(0,
                    personaText.Count));

                    Log.LogDebug("Player personality was randomized into: " +
                        personalityType);

                    if (allowRandomColours.Value == true &&
                        (savedVanillaEyeList == null || savedVanillaEyeList.Count <= 0))
                    {
                        var colour = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f));
                        playerColours.Add("eyes", colour);
                        playerColours.Add("text", colour);
                        thoughtColour = colour;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("There was an issue randomizing personality"
                    + " based on json files: " + ex);
            }
            Random.state = state;
        }

        internal void addTrait(string trait)
        {
            if (trait == null || bodyTraits == null || traitText == null ||
                bodyTraits.Contains(trait)
                )
            { return; }

            traitText.TryGetValue(trait, out var file);
            if (file == null) { return; }

            #region Immediately-applicable traits
            if (trait == "gilled" && !owner.hasGills)
            {
                owner.hasGills = true;
            }
            if (trait == "furred" && !owner.hasFur)
            {
                owner.hasFur = true;
            }
            if (trait == "missingeyeleft" && !owner.eyeGone)
            {
                owner.eyeGone = true;
                if (trait != "eyetornoutleft")
                {
                    myFace.eyeTimeHealed = 350f;
                }
            }
            if (trait == "missingeyeright" && !owner.eyeGone)
            {
                owner.eyeGone = true;
                if (trait != "eyetornoutright")
                {
                    rightEyeHealed = 350f;
                }
            }
            if (trait == "missingeyes" && !owner.bothEyesGone)
            {
                owner.bothEyesGone = true;
                if (trait != "eyestornout")
                {
                    myFace.eyeTimeHealed = 350f;
                    rightEyeHealed = 350f;
                }
            }
            if (trait == "missingjaw" && !owner.disfigured)
            {
                owner.disfigured = true;
            }
            if (trait == "deaf" && owner.hearingLoss < 99f)
            {
                owner.hearingLoss = 100f;
            }
            if (trait == "declawed" && (owner.clawHealth >= 0.1f ||
                owner.clawRegrowTime >= 0f))
            {
                owner.clawHealth = 0f;
                owner.clawRegrowTime = 0f;
            }
            if (trait == "narcoleptic" && owner.sleepCycleSpeed < 1.2f)
            {
                owner.sleepCycleSpeed = 1.2f;
            }
            if (trait == "cantsweat" && owner.canSweat)
            {
                owner.canSweat = false;
            }


            if (trait.EndsWith("milky") && owner.charType != 1)
            {
                trait = "sawian-milky";
                returnToVanillaSprites();
                owner.charType = 1;
            }
            if (trait.EndsWith("dune") && owner.charType != 2)
            {
                trait = "sawian-dune";
                returnToVanillaSprites();
                owner.charType = 2;
            }
            #endregion

            bodyTraits.Add(trait);

            #region implied
            if (file.impliesTraits != null &&
                !string.IsNullOrEmpty(file.impliesTraits.ToString()) &&
                file.impliesTraits.Length > 0)
            {
                foreach (string implied in file.impliesTraits)
                {
                    addTrait(implied);
                }
            }
            #endregion
        }

        internal void resetCWTs()
        {
            if (personalSprites != null && personalSprites.Count > 0)
            {
                returnToVanillaSprites();
            }
            if (myWV != null && myWV.CustomCall().UISpriteCache != null && myWV.CustomCall().UISpriteCache.Count > 0)
            {
                myWV.CustomCall().returnToDefault();
            }
            idTag = -1;
            ownerAge = -1;
            ownerHeight = -1;
            ownerVersion = -1;
            personalityType = null;
            personalitySubtypes.Clear();
            personalName = null;
            playerColours.Clear();
            personalSprites.Clear();
            bodyTraits.Clear();
            currentSpriteCache.Clear();
            codeName = null;
            displayName = null;
            colorblindness = null;
            playerTitle = null;
            lastDialogueKey = null;
            rightEyeHealed = 0f;
            myWV = null;
            myFace = null;
            owner = null;
        }

        internal void verifyTitle()
        {
            if (codeName != null)
            {
                playerTitle = codeName;
            }
            else if (personalName != null)
            {
                playerTitle = personalName;
            }
            else
            {
                playerTitle = personalityType;
            }
        }


        #region autospriting
        internal void reviseColours()
        {
            if (playerColours != null && playerColours.Count > 0)
            {
                if (playerColours.TryGetValue("text", out var textcol) &&
                    owner.talker.text.color != textcol && textcol != null)
                {
                    owner.talker.text.color = textcol;
                    playerColours.Add("originaltextcolour", owner.talker.text.color);
                }

                if (playerColours.TryGetValue("eyes", out var eyecol) &&
                    allowRandomColours.Value == true &&
                    (savedVanillaEyeList == null || savedVanillaEyeList.Count <= 0))
                {
                    RecolourEyes(eyecol);
                }

                bool recolwv = false;
                if (playerColours.TryGetValue("wvstats", out var stats))
                {
                    Color.RGBToHSV(stats, out float hue, out _, out _);
                    myWV.CustomCall().statHue = hue;
                    recolwv = true;
                }
                else if (textcol != null)
                {
                    Color.RGBToHSV(textcol, out float hue, out _, out _);
                    myWV.CustomCall().statHue = hue;
                    recolwv = true;
                }

                if (playerColours.TryGetValue("wvback", out var bg))
                {
                    Color.RGBToHSV(bg, out float hue, out _, out _);
                    myWV.CustomCall().backHue = hue;
                    recolwv = true;
                }
                else if (stats != null)
                {
                    Color.RGBToHSV(stats, out float hue, out _, out _);
                    myWV.CustomCall().backHue = hue;
                    recolwv = true;
                }

                if (playerColours.TryGetValue("wvcrit", out var crit))
                {
                    Color.RGBToHSV(crit, out float hue, out _, out _);
                    myWV.CustomCall().critHue = hue;
                    recolwv = true;
                }

                if (playerColours.TryGetValue("primary", out var primcol))
                {
                    owner.furColors.colorKeys[0].color = primcol;
                }
                if (playerColours.TryGetValue("secondary", out var seccol))
                {
                    owner.furColors.colorKeys[1].color = seccol;
                }
                if (playerColours.TryGetValue("tert", out var tertcol))
                {
                    owner.furColors.colorKeys[2].color = tertcol;
                }

                if (playerColours.TryGetValue("blood", out var blood))
                {
                    owner.furColors.colorKeys[3].color = blood;
                }

                if (recolwv && myWV.CustomCall().UISpriteCache.Count <= 0)
                {
                    myWV.CustomCall().recolourMeBaby();
                }
            }
        }
        public void RecolourEyes(Color eyecol)
        {
            Color.RGBToHSV(eyecol, out float hue, out float _, out float _);

            if (savedVanillaEyeList == null || savedVanillaEyeList.Count <= 0 ||
                !savedVanillaEyeList.ContainsKey("eyeList"))
            {
                savedVanillaEyeList.Add("eyeList", myFace.eyeList);
            }
            for (int i = 0; i < myFace.eyeList.Count(); i++)
            {
                Eye eye = myFace.eyeList[i];

                var texture = eye.front.texture;
                var textwo = eye.back.texture;

                if (!texture.isReadable)
                { Log.LogError("Texture to recolour isn't readable: " + texture.name); }
                RecolourHue(texture, hue);
                RecolourHue(textwo, hue);

                myFace.eyeList[i] = eye;
            }
        }
        public void DecolourEyes()
        {
            if (myFace != null && myFace.eyeList != null &&
                savedVanillaEyeList != null && savedVanillaEyeList.Count > 0 &&
                savedVanillaEyeList.TryGetValue("eyeList", out var list))
            {
                myFace.eyeList = list;
            }
        }

        internal void fullLoadSprites()
        {
            try
            {
                if (playerTitle != null)
                {
                    var path = Path.Combine(gspLocation, "GSPersona", "characters",
                    playerTitle);
                    if (!Directory.Exists(path))
                    {
                        path = Path.Combine(gspLocation, "characters",
                            playerTitle);
                    }
                    if (!Directory.Exists(path))
                    {
                        path = Path.Combine(gspLocation, "Assets", playerTitle);
                    }
                    if (!Directory.Exists(path))
                    {
                        path = Path.Combine(gspLocation, playerTitle);
                    }


                    if ((personalSprites == null || personalSprites.Count <= 0) &&
                        Directory.Exists(path))
                    {
                        loadAllPersonalSprites(path, owner);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Error loading sprites: " + ex);
            }

            if (personalSprites.Count <= 0 || personalSprites == null)
            {
                return;
            }

            try
            {
                foreach (string name in personalSprites.Keys)
                {
                    personalSprites.TryGetValue(
                        name, out Sprite currentSprite);
                    // Log.LogDebug("Current sprite load: " + name + ", aka " + currentSprite.name);

                    if (name.StartsWith("overworld"))
                    {
                        if (currentSprite.name.StartsWith("eye"))
                        {
                            if (savedVanillaEyeList != null &&
                                !savedVanillaEyeList.ContainsKey("eyeList"))
                            {
                                savedVanillaEyeList.Add("eyeList",
                                    myFace.eyeList);
                            }
                            try
                            {
                                #region Eyes
                                Eye eye = new();
                                Sprite eyeFront = null;
                                Sprite eyeBack = null;
                                int ivalue = -1;


                                /*
                                EYELIST CHEAT SHEET
                                    0 = normal open
                                    1 = half open
                                    2 = closed
                                    3 = scared
                                    4 = panic
                                    5 = sad
                                    6 = happy
                                    7 = LOCKED IN
                                */

                                #region normal
                                if (currentSprite.name == "eyeopen")
                                {
                                    eyeFront = currentSprite;
                                    ivalue = 0;
                                }

                                if (currentSprite.name == "eyelookback")
                                {
                                    eyeBack = currentSprite;
                                    ivalue = 0;
                                }
                                if (currentSprite.name == "eyeopenback")
                                {
                                    eyeBack = currentSprite;
                                    ivalue = 0;
                                }
                                #endregion
                                #region half open
                                if (currentSprite.name == "eyehalfopen")
                                { eyeFront = currentSprite; ivalue = 1; }
                                if (currentSprite.name == "eyehalfclosed")
                                { eyeFront = currentSprite; ivalue = 1; }

                                if (currentSprite.name == "eyehalfopenback")
                                { eyeBack = currentSprite; ivalue = 1; }
                                if (currentSprite.name == "eyehalfclosedback")
                                { eyeBack = currentSprite; ivalue = 1; }
                                #endregion
                                #region closed
                                if (currentSprite.name == "eyeclosed")
                                { eyeFront = currentSprite; eyeBack = currentSprite;
                                    ivalue = 2; }
                                #endregion
                                #region scared
                                if (currentSprite.name == "eyescared")
                                { eyeFront = currentSprite; ivalue = 3; }
                                if (currentSprite.name == "eyescaredback")
                                { eyeBack = currentSprite; ivalue = 3; }
                                #endregion
                                #region panic
                                if (currentSprite.name == "eyepanic")
                                { eyeFront = currentSprite; ivalue = 4; }
                                if (currentSprite.name == ("eyepanicback"))
                                { eyeBack = currentSprite; ivalue = 4; }
                                #endregion
                                #region sad
                                if (currentSprite.name == "eyesad")
                                { eyeFront = currentSprite; ivalue = 5; }
                                if (currentSprite.name == "eyesadback")
                                { eyeBack = currentSprite; ivalue = 5; }
                                #endregion
                                #region happy
                                if (currentSprite.name == "eyehappy")
                                { eyeFront = currentSprite; ivalue = 6; }
                                if (currentSprite.name == "eyehappyback")
                                { eyeBack = currentSprite; ivalue = 6; }
                                #endregion
                                #region locked in
                                if (currentSprite.name == "eyelockedin")
                                { eyeFront = currentSprite; ivalue = 7; }
                                if (currentSprite.name == "eyelockedinback")
                                { eyeBack = currentSprite; ivalue = 7; }
                                #endregion

                                if (eyeBack != null)
                                {
                                    eye.back = eyeBack;
                                    eye.front = myFace.eyeList[ivalue].front;
                                }
                                if (eyeFront != null)
                                {
                                    eye.front = eyeFront;
                                    eye.back = myFace.eyeList[ivalue].back;
                                }

                                if (eye.front != null && eye.back != null &&
                                    ivalue != -1)
                                {
                                    myFace.eyeList[ivalue] = eye;
                                }

                                if (myFace != null && myFace.eyesGoneHealed != null &
                                    currentSprite.name == "eyegonehealed")
                                {
                                    if (savedVanillaSpriteSets != null &&
                                        !savedVanillaSpriteSets.ContainsKey("eyesGoneHealed"))
                                    {
                                        savedVanillaSpriteSets.Add("eyesGoneHealed",
                                            [myFace.eyesGoneHealed]);
                                    }
                                    myFace.eyesGoneHealed = currentSprite;
                                }
                                if (myFace != null && myFace.eyesGone != null &&
                                    currentSprite.name == "eyegone")
                                {
                                    if (savedVanillaSpriteSets != null &&
                                        !savedVanillaSpriteSets.ContainsKey("eyesGone"))
                                    {
                                        savedVanillaSpriteSets.Add("eyesGone",
                                            [myFace.eyesGone]);
                                    }
                                    myFace.eyesGone = currentSprite;
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                Log.LogError("Error setting eye sprites: " + ex);
                            }
                        }
                        else if (currentSprite.name.StartsWith("headback"))
                        {
                            if (currentSprite.name == "headbackmouth")
                            {
                                if (savedVanillaSpriteSets != null &&
                                        !savedVanillaSpriteSets.ContainsKey(
                                            "defaultHeadMouth"))
                                {
                                    savedVanillaSpriteSets.Add("defaultHeadMouth",
                                        [myFace.defaultHeadMouth]);
                                }
                                myFace.defaultHeadMouth = currentSprite;
                            }

                            if (currentSprite.name == "headbackmouthmini" ||
                                currentSprite.name == "defaultheadmouthhalf")
                            {
                                if (savedVanillaSpriteSets != null &&
                                        !savedVanillaSpriteSets.ContainsKey(
                                            "defaultHeadMouthHalf"))
                                {
                                    savedVanillaSpriteSets.Add("defaultHeadMouthHalf",
                                        [myFace.defaultHeadMouthHalf]);
                                }
                                myFace.defaultHeadMouthHalf = currentSprite;
                            }

                            if (currentSprite.name == "headback")
                            {
                                if (savedVanillaSpriteSets != null &&
                                    !savedVanillaSpriteSets.ContainsKey(
                                            "defaultHead"))
                                {
                                    savedVanillaSpriteSets.Add("defaultHead",
                                        [myFace.defaultHead]);
                                }
                                myFace.defaultHead = currentSprite;
                            }
                        }
                        else if (myFace != null &&
                            currentSprite.name.StartsWith("headdisfigured"))
                        {
                            try
                            {
                                string unhealed = (currentSprite.name.Replace("healed",
                                    " ")).Trim();

                                if (currentSprite.name.EndsWith("healed") &&
                                    unhealed != null)
                                {
                                    if (savedVanillaSpriteSets != null &&
                                        !savedVanillaSpriteSets.ContainsKey(
                                            "disfiguredHeadHeal"))
                                    {
                                        savedVanillaSpriteSets.Add("disfiguredHeadHeal",
                                            myFace.disfiguredHeadHeal);
                                    }

                                    if (int.TryParse(unhealed.Last().ToString(),
                                        out int dis))
                                    {
                                        myFace.disfiguredHeadHeal[dis - 1] =
                                            currentSprite;
                                    }
                                    else
                                    {
                                        Log.LogWarning("Could not parse index of " +
                                            currentSprite.name);
                                    }
                                }
                                else
                                {
                                    if (savedVanillaSpriteSets != null &&
                                        !savedVanillaSpriteSets.ContainsKey(
                                            "disfiguredHead"))
                                    {
                                        savedVanillaSpriteSets.Add("disfiguredHead",
                                            myFace.disfiguredHead);
                                    }

                                    if (int.TryParse(currentSprite.name.Last().ToString(),
                                        out int dis))
                                    {
                                        myFace.disfiguredHead[dis - 1] = currentSprite;
                                    }
                                    else
                                    {
                                        Log.LogWarning("Could not parse index of " +
                                            currentSprite.name);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.LogError("Error swapping disfigured sprites: " + ex);
                            }
                        }
                        else
                        {
                            foreach (SpriteRenderer spriteRenderer in owner.gameObject.GetComponentsInChildren<SpriteRenderer>())
                            {
                                if (savedVanillaSpriteSets != null &&
                                    !savedVanillaSpriteSets.ContainsKey(
                                        spriteRenderer.sprite.name))
                                {
                                    savedVanillaSpriteSets.Add(
                                        spriteRenderer.sprite.name,
                                        [spriteRenderer.sprite]);
                                }

                                string limbName = spriteRenderer.sprite.name.ToLower();
                                if (!(spriteRenderer.sprite == null) &&
                                        limbName.EndsWith(currentSprite.name))
                                {
                                    spriteRenderer.sprite = currentSprite;
                                }
                            }
                        }
                    }
                    else if (name.StartsWith("woundview"))
                    {
                        if (savedVanillaImageSets != null &&
                            !savedVanillaImageSets.ContainsKey("limbimages"))
                        {
                            savedVanillaImageSets.Add("limbimages",
                                myWV.limbImages);
                        }

                        for (int i = 0; i < myWV.limbImages.Length; i++)
                        {
                            string limbname = getStringFromLimbInt(i);
                            if (limbname + "-outline" == currentSprite.name)
                            {
                                Image originalLimb = myWV.limbImages[i];
                                if (currentSprite != null && originalLimb != null)
                                {
                                    Vector2 initialSize = originalLimb.rectTransform.sizeDelta;
                                    Vector2 initialHeightAndWidth = new(
                                        originalLimb.sprite.rect.width,
                                        originalLimb.sprite.rect.height);

                                    originalLimb.sprite = currentSprite;
                                    originalLimb.rectTransform.sizeDelta = new(
                                        initialSize.x * originalLimb.sprite.rect.width /
                                        initialHeightAndWidth.x,
                                        initialSize.y * originalLimb.sprite.rect.height /
                                        initialHeightAndWidth.y);
                                }
                            }
                            else if (limbname == currentSprite.name)
                            {
                                Image originalLimb = myWV.limbImages[i].rectTransform.GetChild(
                                    0).GetComponent<Image>();
                                if (currentSprite != null && originalLimb != null)
                                {
                                    Vector2 initialSize = originalLimb.rectTransform.sizeDelta;
                                    Vector2 initialHeightAndWidth = new(
                                        originalLimb.sprite.rect.width,
                                        originalLimb.sprite.rect.height);

                                    originalLimb.sprite = currentSprite;
                                    originalLimb.rectTransform.sizeDelta = new(
                                        initialSize.x * originalLimb.sprite.rect.width /
                                        initialHeightAndWidth.x,
                                        initialSize.y * originalLimb.sprite.rect.height /
                                        initialHeightAndWidth.y);
                                }
                            }
                        }

                        #region manual applications
                        if (currentSprite.name == "disfigured")
                        {
                            if (savedVanillaImageSets != null &&
                            !savedVanillaImageSets.ContainsKey("disfiguredImage"))
                            {
                                savedVanillaImageSets.Add("disfiguredImage",
                                    [myWV.disfiguredImage]);
                            }
                            myWV.disfiguredImage.sprite = currentSprite;
                        }
                        if (currentSprite.name == "eyelessleft")
                        {
                            if (savedVanillaImageSets != null &&
                            !savedVanillaImageSets.ContainsKey("eyeGoneImage"))
                            {
                                savedVanillaImageSets.Add("eyeGoneImage",
                                    [myWV.eyeGoneImage]);
                            }
                            myWV.eyeGoneImage.sprite = currentSprite;
                        }
                        if (currentSprite.name == "eyelessright")
                        {
                            if (savedVanillaImageSets != null &&
                            !savedVanillaImageSets.ContainsKey("bothEyesGoneImage"))
                            {
                                savedVanillaImageSets.Add("bothEyesGoneImage",
                                    [myWV.bothEyesGoneImage]);
                            }
                            myWV.bothEyesGoneImage.sprite = currentSprite;
                        }

                        if (bodyTraits.Contains("hidemissingeyeleft") &&
                            bodyTraits.Contains("missingeyeleft"))
                        {
                            myWV.eyeGoneImage.color = new Color(0f, 0f, 0f, 0f);
                        }
                        if (bodyTraits.Contains("hidemissingeyeright") &&
                            bodyTraits.Contains("missingeyeright"))
                        {
                            myWV.bothEyesGoneImage.color = new Color(0f, 0f, 0f, 0f);
                        }

                        if (bodyTraits.Contains("hidemissingeyes") &&
                            bodyTraits.Contains("missingeyes"))
                        {
                            myWV.eyeGoneImage.color = new Color(0f, 0f, 0f, 0f);
                            myWV.bothEyesGoneImage.color = new Color(0f, 0f, 0f, 0f);
                        }
                        #endregion
                    }
                    else if (name.StartsWith("minigame"))
                    {
                        if (savedVanillaSpriteSets != null &&
                            !savedVanillaSpriteSets.ContainsKey("handsprites"))
                        {
                            savedVanillaSpriteSets.Add("handsprites",
                                MinigameBase.main.handSprites);
                        }

                        for (int i = 0; i < MinigameBase.main.handSprites.Length; i++)
                        {
                            if (MinigameBase.main.handSprites[i].name ==
                                currentSprite.name ||
                                MinigameBase.main.handSprites[i].name.Contains(
                                currentSprite.name) // to prevent any issues with furless chars
                                )
                            {
                                MinigameBase.main.handSprites[i] = currentSprite;
                            }
                        }
                    }
                }

                #region eyes
                if (bodyTraits != null &&
                    bodyTraits.Count > 0 &&
                    bodyTraits.Contains("hidemissingeyeleft") ||
                    bodyTraits.Contains("hidemissingeyes"))
                {
                    Color col = myWV.eyeGoneImage.color;
                    col.a = 0f;
                    myWV.eyeGoneImage.color = col;
                }
                if (bodyTraits != null && bodyTraits.Count > 0 &&
                    bodyTraits.Contains("hidemissingeyeright") ||
                    bodyTraits.Contains("hidemissingeyes"))
                {
                    Color col = myWV.bothEyesGoneImage.color;
                    col.a = 0f;
                    myWV.bothEyesGoneImage.color = col;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log.LogError("Error swapping player sprites: " + ex);
            }
        }
        internal void returnToVanillaSprites()
        {
            DecolourEyes();
            if (personalSprites.Count <= 0 || personalSprites == null)
            {
                return;
            }

            myWV.CustomCall().statHue = null;
            myWV.CustomCall().backHue = null;
            myWV.CustomCall().critHue = null;
            myWV.CustomCall().isDirty = true;
            if (playerColours.TryGetValue("originaltextcolour", out var og) && owner.talker.text.color != og)
            {
                owner.talker.text.color = og;
            }

            if (PauseHandler.main != null && PauseHandler.main.pauseText.color != null)
            {
                PauseHandler.main.pauseText.color = Color.white;
            }
            if (thoughtColour != null)
            {
                thoughtColour = new Color();
            }

            if (myFace != null)
            {
                if (savedVanillaEyeList != null &&
                savedVanillaEyeList.TryGetValue("eyeList", out List<Eye> eye))
                {
                    myFace.eyeList = eye;
                }
                if (savedVanillaSpriteSets != null &&
                    savedVanillaSpriteSets.TryGetValue("eyesGoneHealed", out
                    Sprite[] eyegheal)
                        )
                {
                    myFace.eyesGoneHealed = eyegheal.First();
                }
                if (savedVanillaSpriteSets != null &&
                    savedVanillaSpriteSets.TryGetValue("eyesGone", out
                    Sprite[] eyeg)
                        )
                {
                    myFace.eyesGone = eyeg.First();
                }
                if (savedVanillaSpriteSets != null &&
                    savedVanillaSpriteSets.TryGetValue("defaultHeadMouth", out
                    Sprite[] mouth))
                {
                    myFace.defaultHeadMouth = mouth.First();
                }
                if (savedVanillaSpriteSets != null &&
                    savedVanillaSpriteSets.TryGetValue("defaultHeadMouthHalf", out
                    Sprite[] hmouth))
                {
                    myFace.defaultHeadMouthHalf = hmouth.First();
                }
                if (savedVanillaSpriteSets != null &&
                    savedVanillaSpriteSets.TryGetValue("defaultHead", out
                    Sprite[] head))
                {
                    myFace.defaultHead = head.First();
                }
                if (savedVanillaSpriteSets != null &&
                    savedVanillaSpriteSets.TryGetValue("disfiguredHeadHeal", out
                    Sprite[] dheadh))
                {
                    myFace.disfiguredHeadHeal = dheadh;
                }
                if (savedVanillaSpriteSets != null &&
                    savedVanillaSpriteSets.TryGetValue("disfiguredHead", out
                    Sprite[] dhead))
                {
                    myFace.disfiguredHeadHeal = dhead;
                }
            }
            foreach (SpriteRenderer spriteRenderer in owner.gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                if (savedVanillaSpriteSets != null &&
                    savedVanillaSpriteSets.TryGetValue(
                        spriteRenderer.sprite.name, out var sprite))
                {
                    spriteRenderer.sprite = sprite.First();
                }
            }
            if (myWV != null)
            {
                if (savedVanillaImageSets != null &&
                savedVanillaImageSets.TryGetValue("limbimages", out var woundIm))
                {
                    myWV.limbImages = woundIm;
                }
                if (savedVanillaImageSets != null &&
                savedVanillaImageSets.TryGetValue("disfiguredImage", out var dima))
                {
                    myWV.disfiguredImage = dima.First();
                }
                if (savedVanillaImageSets != null &&
                savedVanillaImageSets.TryGetValue("eyeGoneImage", out var eyeL))
                {
                    myWV.eyeGoneImage = eyeL.First();
                }
                if (savedVanillaImageSets != null &&
                savedVanillaImageSets.TryGetValue("bothEyesGoneImage", out var eyeR))
                {
                    myWV.bothEyesGoneImage = eyeR.First();
                }
                if (!bodyTraits.Contains("hidemissingeyeleft") &&
                !bodyTraits.Contains("missingeyeleft"))
                {
                    myWV.eyeGoneImage.color = new Color(0f, 0f, 0f, 0f);
                }
                if (!bodyTraits.Contains("hidemissingeyeright") &&
                    !bodyTraits.Contains("missingeyeright"))
                {
                    myWV.bothEyesGoneImage.color = new Color(1f, 1f, 1f, 1f);
                }
                if (!bodyTraits.Contains("hidemissingeyes") &&
                    !bodyTraits.Contains("missingeyes"))
                {
                    myWV.eyeGoneImage.color = new Color(1f, 1f, 1f, 1f);
                    myWV.bothEyesGoneImage.color = new Color(1f, 1f, 1f, 1f);
                }
            }
            if (savedVanillaSpriteSets != null && savedVanillaSpriteSets.TryGetValue(
                "handsprites", out var hands))
            {
                MinigameBase.main.handSprites = hands;
            }

            savedVanillaEyeList.Clear();
            savedVanillaSpriteSets.Clear();
            savedVanillaImageSets.Clear();
            personalSprites.Clear();
        }
        #endregion


        internal PlayerKind()
        {

        }
    }
    internal static readonly ConditionalWeakTable<Body, PlayerKind> PlayerCWT = new();
    internal static PlayerKind PlayerSelf(this Body body) => PlayerCWT.GetValue(body, _ => new());
}