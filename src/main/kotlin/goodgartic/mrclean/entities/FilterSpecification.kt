package goodgartic.mrclean.entities

/**
 * Convert a comma-separated list of entities, that are either allowed or denied (when prefixed with an !),
 * to a filter specification instance allowing matching entries
 */
fun String.toFilterSpecification(): FilterSpecification =
    if (this.isBlank()) FilterSpecification()
    else this.split(",").partition { it.startsWith("!") }
        .let { (denied, allowed) ->
            FilterSpecification(allowed, denied.map { it.removePrefix("!") })
        }

data class FilterSpecification(
    val allowedEntities: List<String> = emptyList(),
    val deniedEntities: List<String> = emptyList(),
) {
    fun matches(entry: String): Boolean = when (entry) {
        // If the entry was explicitly denied
        in deniedEntities -> false
        // If the entry was explicitly allowed
        in allowedEntities -> true
        // If allowed entities are empty, allow all entities, otherwise limit the specification to whitelist-only
        else -> allowedEntities.isEmpty()
    }
}