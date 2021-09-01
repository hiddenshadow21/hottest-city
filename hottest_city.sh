#!/bin/bash

cd $GET_CURRENT_WEATHER_BIN

outputFile="tmpOutput.txt"
[ -f $outputFile ] && rm $outputFile 

declare -A tasksIds
while IFS= read -r line; do
    ./GetCurrentWeather -c "$line" >> $outputFile  &
    tasksIds[$!]="$line"
done

#read is ignoring last line, that's why I check if it's not empty
if [ -n "$line" ]; then
    ./GetCurrentWeather -c "$line" >> $outputFile  &
    tasksIds[$!]="$line"
fi

for taskId in ${!tasksIds[@]}; do
    wait $taskId
    [ $? -ne 0 ] && echo "Could not pull the weather info for ${tasksIds[$taskId]}"
done

gawk -F'|' 'BEGIN{
    print "Three hottest cities are:"
}
{
    print NR". " $1 " ("$2"C)"
}' <(sort -k2nr -t '|' $outputFile | head -n 3)

rm $outputFile