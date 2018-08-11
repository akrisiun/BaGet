docker rm -f nuget-5555
docker build . -t baget-1
docker run -it -d --name nuget-5555 -p 5555:80 --env-file baget.env -v /Users/andriusk/Beta/dot.source/BaGet/baget-data:/var/baget baget-1

docker ps | grep nuget
