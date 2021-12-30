package goodgartic.mrclean.discord.commands

import net.dv8tion.jda.api.entities.Guild
import net.dv8tion.jda.api.hooks.ListenerAdapter
import org.springframework.stereotype.Component

@Component
class SlashCommandListener(private val guild: Guild, private val commands: List<SlashCommand>) : ListenerAdapter() {

    init {
        registerCommands()
    }

    private fun registerCommands() {
        guild.updateCommands()
            .addCommands(commands.map { it.definition })
            .queue()
    }
}