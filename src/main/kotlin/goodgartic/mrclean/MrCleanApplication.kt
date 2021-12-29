package goodgartic.mrclean

import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.runApplication

@SpringBootApplication
class MrCleanApplication

fun main(args: Array<String>) {
    runApplication<MrCleanApplication>(*args)
}
