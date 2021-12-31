package goodgartic.mrclean

import goodgartic.mrclean.service.DiscordService
import org.springframework.boot.CommandLineRunner
import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.context.properties.ConfigurationPropertiesScan
import org.springframework.boot.runApplication
import org.springframework.scheduling.annotation.EnableScheduling

@SpringBootApplication
@EnableScheduling
@ConfigurationPropertiesScan("goodgartic.mrclean.configuration")
class MrCleanApplication(private val service: DiscordService) : CommandLineRunner {
    override fun run(vararg args: String?) = service.start()
}

fun main(args: Array<String>) {
    runApplication<MrCleanApplication>(*args)
}
