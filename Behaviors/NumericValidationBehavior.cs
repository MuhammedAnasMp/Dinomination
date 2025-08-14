using Microsoft.Maui.Controls;
using System.Linq;

namespace Denomination.Behaviors
{
    public class NumericValidationBehavior : Behavior<Entry>
    {
        public bool AllowDecimal { get; set; } = true;
        public bool AllowNegative { get; set; } = false;

        // Placeholder text
        private const string PlaceholderText = "Enter quantity";

        protected override void OnAttachedTo(Entry entry)
        {
            entry.Placeholder = PlaceholderText;
            entry.Keyboard = Keyboard.Numeric; // Forces numeric keyboard on mobile
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = (Entry)sender;
            string newText = args.NewTextValue ?? "";
            string oldText = args.OldTextValue ?? "";

            // If cleared → set to 0
            if (string.IsNullOrWhiteSpace(newText))
            {
                entry.Text = "0";
                return;
            }

            // Prevent just decimal
            if (newText == ".")
            {
                entry.Text = "0.";
                return;
            }

            // Validate
            if (!CheckIsValidNumber(newText, AllowDecimal, AllowNegative))
            {
                entry.Text = oldText; // revert to previous valid text
            }
        }

        private bool CheckIsValidNumber(string text, bool allowDecimal, bool allowNegative)
        {
            // Handle negative
            if (text.StartsWith("-"))
            {
                if (!allowNegative) return false;
                text = text.Substring(1);
                if (string.IsNullOrEmpty(text)) return false;
            }

            // Decimal handling
            if (allowDecimal)
            {
                if (text.Count(c => c == '.') > 1) return false;
            }
            else if (text.Contains('.'))
            {
                return false;
            }

            // Must be digits or decimal
            foreach (char c in text)
            {
                if (!char.IsDigit(c) && c != '.')
                    return false; // blocks special characters & letters
            }

            return true;
        }
    }
}
