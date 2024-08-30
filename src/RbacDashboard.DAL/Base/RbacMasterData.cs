using RbacDashboard.DAL.Models;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Base;

[ExcludeFromCodeCoverage(Justification = "This class is used like a constant, so there's no need to test it.")]
public static class RbacMasterData
{
    public static List<OptionsetMaster> OptionsetMasters => [
        new OptionsetMaster {
            Id = Guid.Parse("a05fb322-eec1-400d-8edf-0c08847ac434"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Role",
            Value = Guid.Parse("abb332d7-c6c0-4b8e-b4f0-6d58e1888858"),
            JsonObject = string.Empty
        },
        new OptionsetMaster {
            Id = Guid.Parse("deaf670f-a562-4020-bfed-3ecb137f6aeb"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Access",
            Value = Guid.Parse("c1c61226-55c5-49ea-9dab-da0b738703cd"),
            JsonObject = string.Empty
        },
        new OptionsetMaster {
            Id = Guid.Parse("fc6b96fb-46d2-4b2c-bb99-acc18cfcc209"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Navigation",
            Value = Guid.Parse("4b9538ff-4305-4816-aac8-87aa6d83259f"),
            JsonObject = string.Empty
        },
        new OptionsetMaster {
            Id = Guid.Parse("2fa9037f-8cf1-42b5-87d1-bef5aa3fc341"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "EndPoind",
            Value = Guid.Parse("a3d08e9a-8600-451d-8fcf-72a9d1773bc0"),
            JsonObject = string.Empty
        }
    ];

    public static List<TypeMaster> TypeMasters => [
        new TypeMaster {
            Id = Guid.Parse("b004eef6-578f-46ac-9f0c-13e50c0d7a3f"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "EndPoint",
            OptionsetMasterId = Guid.Parse("deaf670f-a562-4020-bfed-3ecb137f6aeb")
        },
        new TypeMaster {
            Id = Guid.Parse("553ac0ff-2708-418f-8c11-1945cb7c5f78"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Application",
            OptionsetMasterId = Guid.Parse("deaf670f-a562-4020-bfed-3ecb137f6aeb")
        },
        new TypeMaster {
            Id = Guid.Parse("eaf72508-1cda-40a4-9a62-2a4c6d6b5fb9"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Customer",
            OptionsetMasterId = Guid.Parse("deaf670f-a562-4020-bfed-3ecb137f6aeb")
        },
        new TypeMaster {
            Id = Guid.Parse("48cb670d-8084-4508-81ab-2c17854519b6"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Company",
            OptionsetMasterId = Guid.Parse("a05fb322-eec1-400d-8edf-0c08847ac434")
        },
        new TypeMaster {
            Id = Guid.Parse("984d2f67-36fc-47b2-8c90-52e838daa88d"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "BusinessUnit",
            OptionsetMasterId = Guid.Parse("deaf670f-a562-4020-bfed-3ecb137f6aeb")
        },
        new TypeMaster {
            Id = Guid.Parse("dee990a2-6fef-4ad4-b560-c1f32c701677"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Role",
            OptionsetMasterId = Guid.Parse("deaf670f-a562-4020-bfed-3ecb137f6aeb")
        }
    ];

    public static List<Permissionset> Permissionsets => [
        new Permissionset {
            Id = Guid.Parse("8350b797-4383-45c5-93ff-6b5e434151ac"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Read",
            PermissionTypeId = Guid.Parse("a05fb322-eec1-400d-8edf-0c08847ac434"),
            ParentId = null
        },
        new Permissionset {
            Id = Guid.Parse("8350b797-4383-45c5-93ff-6b5e434151ad"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Write",
            PermissionTypeId = Guid.Parse("a05fb322-eec1-400d-8edf-0c08847ac434"),
            ParentId = null
        },
        new Permissionset {
            Id = Guid.Parse("8350b797-4383-45c5-93ff-6b5e434151ae"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Update",
            PermissionTypeId = Guid.Parse("a05fb322-eec1-400d-8edf-0c08847ac434"),
            ParentId = null
        },
        new Permissionset {
            Id = Guid.Parse("8350b797-4383-45c5-93ff-6b5e434151af"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Delete",
            PermissionTypeId = Guid.Parse("a05fb322-eec1-400d-8edf-0c08847ac434"),
            ParentId = null
        },
        new Permissionset {
            Id = Guid.Parse("8350b797-4383-45c5-93ff-6b5e434151ba"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Navigate",
            PermissionTypeId = Guid.Parse("fc6b96fb-46d2-4b2c-bb99-acc18cfcc209"),
            ParentId = null
        },
        new Permissionset {
            Id = Guid.Parse("8350b797-4383-45c5-93ff-6b5e434151bb"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Exceute",
            PermissionTypeId = Guid.Parse("2fa9037f-8cf1-42b5-87d1-bef5aa3fc341"),
            ParentId = null
        },
        new Permissionset {
            Id = Guid.Parse("8350b797-4383-45c5-93ff-6b5e434151bc"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Share",
            PermissionTypeId = Guid.Parse("deaf670f-a562-4020-bfed-3ecb137f6aeb"),
            ParentId = null
        },
        new Permissionset {
            Id = Guid.Parse("8350b797-4383-45c5-93ff-6b5e434151bd"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "Access",
            PermissionTypeId = Guid.Parse("deaf670f-a562-4020-bfed-3ecb137f6aeb"),
            ParentId = null
        },
        new Permissionset {
            Id = Guid.Parse("8350b797-4383-45c5-93ff-6b5e434151bf"),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            IsDeleted = false,
            Name = "API",
            PermissionTypeId = Guid.Parse("2fa9037f-8cf1-42b5-87d1-bef5aa3fc341"),
            ParentId = null
        }
    ];
}