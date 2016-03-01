function passwordStrength(password) {
    if (password.length > 5) {
        var desc = new Array();
        desc[1] = "Weak";
        desc[2] = "Medium";
        desc[3] = "Strong";
        desc[4] = "Very Strong";

        var charscore = 0, passwordscore = 0, score = 0;

        if (password.length > 0) {
            passwordscore++;
        }

        //if password bigger than 8 give 1 point
        if (password.length > 8) passwordscore++;

        //if password between 8 and 12 give 1 point
        if (password.length >= 8 && password.length <= 12) passwordscore++;

        //if password bigger than 12 give 2 point
        if (password.length > 12) {
            passwordscore = passwordscore + 2;
        }

        //if password has uppercase characters give 1 point	
        if (password.match(/[A-Z]/)) charscore++;

        //if password has lowercase characters give 1 point	
        if (password.match(/[a-z]/)) charscore++;

        //if password has at least one number give 1 point
        if (password.match(/\d+/)) charscore++;

        //if password has at least one special caracther give 1 point
        if (password.match(/.[!,@,#,$,%,^,&,*,?,_,~,-,(,)]/)) charscore++;

        //Weak
        if ((passwordscore > 0 && passwordscore < 2) && (charscore <= 3)) {
            score = 1;
        } else //Strong
            if (passwordscore == 3 && charscore == 4) {
                score = 3;
            } else //Very Strong
                if (passwordscore == 4 && charscore == 4) {
                    score = 4;
                } else //Medium
                    if (passwordscore > 2 && charscore >= 3) {
                        score = 2;
                    } else {
                        score = 1;
                    }

        document.getElementById("passwordDescription").innerHTML = desc[score];
        document.getElementById("passwordStrength").className = "strength" + score;

    } else {
        document.getElementById("passwordDescription").innerHTML = "";
        document.getElementById("passwordStrength").className = "";
    }
}