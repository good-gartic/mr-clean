package goodgartic.mrclean.discord.commands

import net.dv8tion.jda.api.events.interaction.SlashCommandEvent
import net.dv8tion.jda.api.interactions.commands.build.CommandData

interface SlashCommand {

    val definition: CommandData

    fun execute(event: SlashCommandEvent)

}