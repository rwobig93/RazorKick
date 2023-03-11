﻿namespace Shared.Responses.Identity;

public class PasswordRequirementsResponse
{
    public int MinimumLength { get; set; }
    public bool RequiresSpecialCharacters { get; set; }
    public bool RequiresLowercase { get; set; }
    public bool RequiresUppercase { get; set; }
    public bool RequiresNumbers { get; set; }
}