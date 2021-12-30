package goodgartic.mrclean.commands

import net.dv8tion.jda.api.entities.Guild
import net.dv8tion.jda.api.events.interaction.SlashCommandEvent
import net.dv8tion.jda.api.hooks.ListenerAdapter
import org.springframework.stereotype.Component
import java.lang.IllegalStateException

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

    override fun onSlashCommand(event: SlashCommandEvent) {
        val handler = commands.firstOrNull { command -> command.definition.name == event.name }
            ?: throw IllegalStateException("Cannot find handler for slash command /${event.name}")

            // TODO: Add exception handling, permissions stuff etc...
            handler.execute(event)
    }
}