using System.Text.RegularExpressions;

namespace Api.Auth
{
    public class PasswordValidator
    {
        private const int MinLength = 8;
        private const string UpperCasePattern = @"[A-Z]";
        private const string LowerCasePattern = @"[a-z]";
        private const string NumberPattern = @"\d";
        private const string SpecialCharPattern = @"[!@#$%^&*()_+=\[\]{};':\|,.<>\\/?-]";

        public static (bool IsValid, List<string> Errors) ValidatePassword(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password) || password.Length < MinLength)
                errors.Add($"Password must be at least {MinLength} characters long");

            if (!Regex.IsMatch(password, UpperCasePattern))
                errors.Add("Password must contain at least one uppercase letter");

            if (!Regex.IsMatch(password, LowerCasePattern))
                errors.Add("Password must contain at least one lowercase letter");

            if (!Regex.IsMatch(password, NumberPattern))
                errors.Add("Password must contain at least one number");

            if (!Regex.IsMatch(password, SpecialCharPattern))
                errors.Add("Password must contain at least one special character (!@#$%^&* etc.)");

            return (errors.Count == 0, errors);
        }
    }
}
