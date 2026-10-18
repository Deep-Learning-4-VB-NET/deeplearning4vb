"""Driver: restore the converter text of the listed files from git HEAD (their unported/ path) and convert them.

Usage: python run_convert.py LISTFILE [--report report.txt] [--out DIR]
LISTFILE holds paths relative to nd4j-api's org/nd4j/linalg/api/ops/impl (one per line), or full
paths relative to org/nd4j/ when they start with "org/". The converted files go to the mirrored
path under src/DeepLearning4VB/org/nd4vb/ (or under DIR/org/nd4vb/ for a dry run with --out). Files listed in SKIP (hand-edited after conversion) are left alone.
"""
import pathlib
import subprocess
import sys

sys.path.insert(0, str(pathlib.Path(__file__).parent))
import convert_ops  # noqa: E402

ROOT = convert_ops.ROOT
API = "unported/nd4j/nd4j-backends/nd4j-api-parent/nd4j-api/src/main/java/org/nd4j/"
SKIP_FILE = pathlib.Path(__file__).parent / "hand_edited.txt"


def main(argv):
    lst = pathlib.Path(argv[0])
    report_path = argv[argv.index("--report") + 1] if "--report" in argv else None
    out_root = pathlib.Path(argv[argv.index("--out") + 1]) if "--out" in argv else ROOT / "src" / "DeepLearning4VB"
    skip = set()
    if SKIP_FILE.exists():
        skip = {l.strip() for l in SKIP_FILE.read_text().splitlines() if l.strip()}
    files = []
    for rel in lst.read_text().splitlines():
        rel = rel.strip()
        if not rel:
            continue
        if not rel.startswith("org/"):
            rel = "linalg/api/ops/impl/" + rel
        else:
            rel = rel[len("org/"):]
        if rel in skip:
            continue
        src = subprocess.run(["git", "show", "HEAD:" + API + rel], cwd=ROOT, capture_output=True, check=True).stdout
        dest = out_root / "org" / "nd4vb" / rel
        dest.parent.mkdir(parents=True, exist_ok=True)
        dest.write_bytes(src)
        files.append(str(dest))
    args = files + (["--report", report_path] if report_path else [])
    convert_ops.main(args)


if __name__ == "__main__":
    main(sys.argv[1:])
