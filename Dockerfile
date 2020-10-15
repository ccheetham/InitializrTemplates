FROM alpine as build
RUN apk add zip
COPY bin/ ./bin/
COPY src/ ./src/
RUN bin/build

FROM nginx
COPY --from=build build/ /usr/share/initializr/templates/
COPY deploy/docker/initializr-templates.conf /etc/nginx/conf.d/default.conf
