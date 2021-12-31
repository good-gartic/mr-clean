package goodgartic.mrclean.commands

import goodgartic.mrclean.configuration.Constants
import net.dv8tion.jda.api.EmbedBuilder
import net.dv8tion.jda.api.entities.Guild
import net.dv8tion.jda.api.events.interaction.SlashCommandEvent
import net.dv8tion.jda.api.hooks.ListenerAdapter
import net.dv8tion.jda.api.interactions.components.Button
import org.apache.commons.lang3.exception.ExceptionUtils
import org.springframework.stereotype.Component
import java.io.File

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
        try {
            handler.execute(event)
        }
        catch (exception: Throwable) {
            val embed = EmbedBuilder()
                .setTitle("Oh no, something went wrong")
                .setColor(Constants.Colors.red)
                .setDescription("A `${exception::class.simpleName}` was thrown during the command execution!")
                .setFooter("If this doesn't make any sense to you, simply don't worry about it. It's not you, it's us")
                .build()

            val file = File.createTempFile("stacktrace_", ".txt")
            val stacktrace = ExceptionUtils.getStackTrace(exception)

            file.writeText(stacktrace)

            if (event.isAcknowledged) {
                return event.hook.editOriginalEmbeds(embed)
                    .setActionRow(Button.link("https://github.com/good-gartic/mr-clean/issues/new", "Report a new issue"))
                    .addFile(file)
                    .queue()
            }

            event.replyEmbeds(embed)
                .addActionRow(Button.link("https://github.com/good-gartic/mr-clean/issues/new", "Report a new issue"))
                .addFile(file)
                .queue()
        }
    }
}