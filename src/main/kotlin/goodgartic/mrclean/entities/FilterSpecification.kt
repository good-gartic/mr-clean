package goodgartic.mrclean.entities

/**
 * Convert a comma-separated list of entities, that are either allowed or denied (when prefixed with an !),
 * to a filter specification instance allowing matching entries
 */
fun String.toFilterSpecification(): FilterSpecification =
    if (this.isBlank()) FilterSpecification()
    else this.split(",").partition { it.startsWith("!") }
        .let { (denied, allowed) ->
            FilterSpecification(
                allowed.toSet(),
                denied.map { it.removePrefix("!") }.toSet()
            )
        }

data class FilterSpecification(
    val allowedEntities: Set<String> = emptySet(),
    val deniedEntities: Set<String> = emptySet(),
) {
    fun isEmpty(): Boolean = allowedEntities.isEmpty() && deniedEntities.isEmpty()

    fun matches(entry: String): Boolean = when (entry) {
        // If the entry was explicitly denied
        in deniedEntities -> false
        // If the entry was explicitly allowed
        in allowedEntities -> true
        // If allowed entities are empty, allow all entities, otherwise limit the specification to whitelist-only
        else -> allowedEntities.isEmpty()
    }

    fun allow(entry: String): FilterSpecification = copy(
        allowedEntities = allowedEntities + entry,
        deniedEntities = deniedEntities - entry
    )

    fun deny(entry: String): FilterSpecification = copy(
        allowedEntities = allowedEntities - entry,
        deniedEntities = deniedEntities + entry
    )

    override fun toString(): String {
        val entries = allowedEntities + deniedEntities.map { "!$it" }
        return entries.joinToString(",")
    }
}