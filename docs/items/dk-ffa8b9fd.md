---
id: dk-ffa8b9fd
type: bug
created: 2026-10-05
status: dropped
since: 2026-10-05
area:
priority: P3
rank: n
parent:
fixes: []
blocked_by: []
relates: []
---
# Pickup/harvest animation trigger has no clip on form Animator (cosmetic, won't-fix)

_Moved from docs/BACKLOG.md ("Known cosmetic (won't-fix, documented)")._

The pickup/harvest animation trigger still fires on the form body's Animator, which has no clip
wired for it, so Unity ignores it — no animation, no error. Item still lands in inventory.
