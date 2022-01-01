FROM gradle:alpine AS build

RUN mkdir /build

COPY src                 /build/src
COPY gradlew             /build/gradlew
COPY gradle              /build/gradle
COPY build.gradle.kts    /build/build.gradle.kts
COPY settings.gradle.kts /build/settings.gradle.kts

WORKDIR /build

RUN ./gradlew bootJar


FROM openjdk:17-alpine

RUN mkdir /app
RUN mkdir /app/data
COPY --from=build /build/build/libs/mr-clean-*.jar /app/mr-clean.jar

ENTRYPOINT ["java", "-jar", "/app/mr-clean.jar"]