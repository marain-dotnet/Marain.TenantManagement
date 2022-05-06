Feature: CommonIndirectAndDoublyIndirectDependency

A short summary of the feature


#TODO: what about when there are multiple dependencies on the same service? E.g.:
# Main app uses:
#   Operations
#   Workflow -> Operations
#   Foobar -> Workflow -> Operations
#
# Currently, all three uses of Operations end up with the same config, because it's going to look like:
