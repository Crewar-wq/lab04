#!/usr/bin/env python3
"""Fail unless every production line and branch in both assemblies is covered."""
import argparse
import glob
import os
from decimal import Decimal
from pathlib import Path
import xml.etree.ElementTree as ET

parser = argparse.ArgumentParser()
parser.add_argument("pattern", nargs="?", default="artifacts/coverage/**/coverage.cobertura.xml")
args = parser.parse_args()
paths = glob.glob(args.pattern, recursive=True)
if len(paths) != 1:
    raise SystemExit(f"Expected exactly one coverage report, found {len(paths)}")
root = ET.parse(paths[0]).getroot()
packages = {x.attrib["name"] for x in root.findall("packages/package")}
if packages != {"Lab04.Core", "Lab04.App"}:
    raise SystemExit(f"Unexpected coverage scope: {sorted(packages)}")
lines = int(root.attrib["lines-covered"])
all_lines = int(root.attrib["lines-valid"])
branches = int(root.attrib["branches-covered"])
all_branches = int(root.attrib["branches-valid"])
message = f"Lines: {lines}/{all_lines}; branches: {branches}/{all_branches}"
print(message)
if os.environ.get("GITHUB_STEP_SUMMARY"):
    with open(os.environ["GITHUB_STEP_SUMMARY"], "a", encoding="utf-8") as out:
        out.write("## Coverage\n\n" + message + "\n")
if all_lines == 0 or all_branches == 0 or lines != all_lines or branches != all_branches:
    raise SystemExit("Coverage threshold failed: both lines and branches must equal 100%")
if Decimal(root.attrib["line-rate"]) != 1 or Decimal(root.attrib["branch-rate"]) != 1:
    raise SystemExit("Coverage rates are inconsistent with counters")
