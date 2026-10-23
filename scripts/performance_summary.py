#!/usr/bin/env python3
import json
import os
import sys
from pathlib import Path

report = json.loads(Path(sys.argv[1]).read_text())
text = "## Performance\n\n"
text += f"{report['Framework']}; {report['OS']}; {report['Iterations']:,} iterations; "
text += f"{report['Repeats']} repeats, median.\n\n"
text += "| Method | Median ms | ns/op | B/op |\n|---|---:|---:|---:|\n"
for row in report["Results"]:
    text += f"| {row['Name']} | {row['MedianMilliseconds']:.3f} | "
    text += f"{row['NanosecondsPerOperation']:.3f} | {row['AllocatedBytesPerOperation']:.3f} |\n"
text += "\nMicrobenchmark includes delegate and loop overhead. Timings are environment dependent.\n"
print(text)
Path("artifacts/performance.md").write_text(text)
if os.environ.get("GITHUB_STEP_SUMMARY"):
    with open(os.environ["GITHUB_STEP_SUMMARY"], "a", encoding="utf-8") as out:
        out.write(text)
