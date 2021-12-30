package goodgartic.mrclean.discord

import goodgartic.mrclean.configuration.BotConfiguration
import net.dv8tion.jda.api.JDA
import net.dv8tion.jda.api.JDABuilder
import org.springframework.context.annotation.Bean
import org.springframework.stereotype.Component

@Component
class DiscordBeansProvider(private val configuration: BotConfiguration) {

    @Bean
    fun jda(): JDA = JDABuilder.createDefault(configuration.token).build()

}