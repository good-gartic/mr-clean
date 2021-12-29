package goodgartic.mrclean

import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.context.properties.ConfigurationPropertiesScan
import org.springframework.boot.runApplication

@SpringBootApplication
@ConfigurationPropertiesScan("goodgartic.mrclean.configuration")
class MrCleanApplication

fun main(args: Array<String>) {
    runApplication<MrCleanApplication>(*args)
}
