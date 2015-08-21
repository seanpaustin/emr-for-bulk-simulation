while read line
do
  mono Engine.exe $line
done < "${1:-/dev/stdin}"
