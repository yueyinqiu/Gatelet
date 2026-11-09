{
  description = "Gatelet development";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs =
    { nixpkgs, ... }:
    let
      forAllSystems = nixpkgs.lib.genAttrs nixpkgs.lib.systems.flakeExposed;
    in
    {
      devShells = forAllSystems (
        system:
        let
          pkgs = nixpkgs.legacyPackages.${system};
        in
        {
          default = pkgs.mkShell {
            packages = [
              pkgs.dotnetCorePackages.sdk_10_0
              (pkgs.writeShellScriptBin "gatelet-dev-publish" ''
                set -euo pipefail
                mkdir -p publish
                temp=$(mktemp -d -p publish)

                dotnet publish src/Gatelet/Gatelet.csproj -c Release -o "$temp/Gatelet"
                echo "Don't forget to update the nix package!"
              '')
            ];
            shellHook = ''
              export DOTNET_ROOT="${pkgs.dotnetCorePackages.sdk_10_0}/share/dotnet"
            '';
          };
        }
      );
    };
}