#!/usr/bin/env bash
# Post-deploy smoke checks against a running Cards API instance.
#
# Usage:
#   ./deploy/smoke-test.sh
#   BASE_URL=http://localhost:8080 ./deploy/smoke-test.sh
#
set -euo pipefail

BASE_URL="${BASE_URL:-http://localhost:5202}"
TIMEOUT="${SMOKE_TIMEOUT:-10}"
# User1 sample PANs (see SampleCardPans in Domain)
SAMPLE_PAN="${SAMPLE_PAN:-4010000000000174}"
UNKNOWN_PAN="${UNKNOWN_PAN:-4010000000000992}"

pass() { echo "✓ $1"; }
fail() { echo "✗ $1" >&2; exit 1; }

curl_check() {
  local name="$1"
  local url="$2"
  local expected="${3:-200}"

  local code
  code=$(curl -sS -o /dev/null -w "%{http_code}" --max-time "$TIMEOUT" "$url") \
    || fail "$name — request failed ($url)"

  [[ "$code" == "$expected" ]] \
    || fail "$name — expected HTTP $expected, got $code ($url)"

  pass "$name"
}

echo "Smoke testing $BASE_URL"
echo

curl_check "health" "$BASE_URL/health"
curl_check "health/live" "$BASE_URL/health/live"
curl_check "health/ready" "$BASE_URL/health/ready"

body=$(curl -sS --max-time "$TIMEOUT" \
  "$BASE_URL/users/User1/cards/$SAMPLE_PAN/allowed-actions") \
  || fail "allowed-actions — request failed"

echo "$body" | grep -q '"userId"' \
  && echo "$body" | grep -q '"allowedActions"' \
  || fail "allowed-actions — unexpected response body"

pass "allowed-actions — returns userId and allowedActions"

curl_check "unknown card" \
  "$BASE_URL/users/User1/cards/$UNKNOWN_PAN/allowed-actions" \
  "404"

curl_check "whitespace userId" \
  "$BASE_URL/users/%20/cards/$SAMPLE_PAN/allowed-actions" \
  "400"

echo
echo "All smoke tests passed."
