package goodgartic.mrclean.configuration

import goodgartic.mrclean.configuration.BotConfiguration
import net.dv8tion.jda.api.JDA
import net.dv8tion.jda.api.JDABuilder
import net.dv8tion.jda.api.entities.Guild
import org.springframework.context.annotation.Bean
import org.springframework.stereotype.Component

@Component
final class DiscordBeansProvider(configuration: BotConfiguration) {

    // This class has to be final, as `DiscordBeansProvider#jda` is accessed within the constructor

    @get:Bean
    val jda: JDA = JDABuilder.createDefault(configuration.token).build()

    @get:Bean
    val guild: Guild = jda.getGuildById(configuration.guild)
        ?: throw IllegalStateException("Cannot find the configured guild (${configuration.guild})!")
}