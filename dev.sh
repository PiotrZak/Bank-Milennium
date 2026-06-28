#!/usr/bin/env bash
# Cards API — local development helper
#
# Usage:
#   ./dev.sh              # show help
#   ./dev.sh all          # restore, build, test
#   ./dev.sh run          # start API (foreground)
#   ./dev.sh docker       # build & run in Docker
#   ./dev.sh smoke        # curl smoke checks (API must be running)
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$ROOT"

SLN="Cards.sln"
API_PROJECT="src/Cards.Api/Cards.Api.csproj"
COMPOSE_FILE="deploy/docker-compose.yml"
SMOKE_SCRIPT="deploy/smoke-test.sh"
LOCAL_URL="http://localhost:5202"
DOCKER_URL="http://localhost:8080"

info()  { echo "→ $*"; }
ok()    { echo "✓ $*"; }
die()   { echo "✗ $*" >&2; exit 1; }

require_dotnet() {
  command -v dotnet >/dev/null 2>&1 || die ".NET SDK not found. Install .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0"
  if ! dotnet --list-runtimes 2>/dev/null | grep -q "Microsoft.AspNetCore.App 8."; then
    die "Microsoft.AspNetCore.App 8.0 runtime missing. Install .NET 8 SDK (includes ASP.NET Core 8). You have .NET Core 8 runtime only — that is not enough for this web API."
  fi
}

cmd_restore() {
  require_dotnet
  info "Restoring packages…"
  dotnet restore "$SLN"
  ok "Restore complete"
}

cmd_build() {
  require_dotnet
  info "Building solution…"
  dotnet build "$SLN" --no-restore 2>/dev/null || { dotnet restore "$SLN"; dotnet build "$SLN"; }
  ok "Build complete"
}

cmd_test() {
  require_dotnet
  info "Running all tests…"
  dotnet test "$SLN" --no-build 2>/dev/null || { cmd_build; dotnet test "$SLN"; }
  ok "All tests passed"
}

cmd_test_smoke() {
  require_dotnet
  info "Running in-process smoke tests…"
  dotnet test "$SLN" --filter "Category=Smoke"
  ok "Smoke tests passed"
}

cmd_run() {
  require_dotnet
  cmd_build
  info "Starting API at $LOCAL_URL (Ctrl+C to stop)…"
  echo "  Swagger: $LOCAL_URL/swagger"
  dotnet run --project "$API_PROJECT" --no-build
}

cmd_docker() {
  command -v docker >/dev/null 2>&1 || die "Docker not found"
  info "Building and starting Docker stack…"
  docker compose -f "$COMPOSE_FILE" up --build
}

cmd_docker_down() {
  command -v docker >/dev/null 2>&1 || die "Docker not found"
  info "Stopping Docker stack…"
  docker compose -f "$COMPOSE_FILE" down
  ok "Docker stack stopped"
}

cmd_smoke() {
  command -v curl >/dev/null 2>&1 || die "curl not found"
  local base_url="${BASE_URL:-$LOCAL_URL}"
  info "Live smoke tests against ${base_url}..."
  BASE_URL="$base_url" bash "$SMOKE_SCRIPT"
}

cmd_all() {
  cmd_restore
  cmd_build
  cmd_test
  ok "Ready — run './dev.sh run' or './dev.sh docker'"
}

cmd_dev() {
  cmd_all
  cmd_run
}

usage() {
  cat <<EOF
Cards API — dev helper

Usage: ./dev.sh <command>

Commands:
  all          Restore, build, and run all tests (default CI check)
  dev          Same as 'all', then start the API locally
  restore      dotnet restore
  build        Build the solution
  test         Run all tests
  test-smoke   Run in-process smoke tests (fast)
  run          Build and start API on $LOCAL_URL
  docker       docker compose up --build ($DOCKER_URL)
  docker-down  Stop Docker stack
  smoke        Live curl smoke tests (set BASE_URL, default $LOCAL_URL)

Examples:
  ./dev.sh all
  ./dev.sh run
  BASE_URL=$DOCKER_URL ./dev.sh smoke

EOF
}

main() {
  local cmd="${1:-help}"
  shift || true

  case "$cmd" in
    help|-h|--help)   usage ;;
    all|check|ci)     cmd_all "$@" ;;
    dev)              cmd_dev "$@" ;;
    restore)          cmd_restore "$@" ;;
    build)            cmd_restore; cmd_build "$@" ;;
    test)             cmd_test "$@" ;;
    test-smoke|smoke-tests) cmd_test_smoke "$@" ;;
    run|start)        cmd_run "$@" ;;
    docker|up)        cmd_docker "$@" ;;
    docker-down|down) cmd_docker_down "$@" ;;
    smoke)            cmd_smoke "$@" ;;
    *)                die "Unknown command: $cmd (try ./dev.sh help)" ;;
  esac
}

main "$@"
