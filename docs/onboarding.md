# Your first week

## Day one

1. Accept the organisation invitation. You land in `@gh-training-ak/frontend-team`
   or whichever team matches your squad, which is what gives you write access.
2. Open the repository in the dev container. Everything is pinned there, so you do
   not spend the morning installing the wrong .NET version.
3. Run `dotnet test Contoso.sln`. 54 tests, about a second. If that does not work,
   stop and ask, do not start changing things.

## Day two

Read, in this order:

1. [`docs/architecture.md`](architecture.md), specifically the dependency rule.
2. [`docs/adr/README.md`](adr/README.md), skim the titles, read 0002 and 0003 in full.
3. [`.github/CONTRIBUTING.md`](../.github/CONTRIBUTING.md), the merge button table.

## Day three to five

Pick an issue labelled `good first issue`. Open a draft pull request early, even when
it is wrong. Nobody here reviews a finished branch for the first time.

## Things that will trip you up

- `Money` is not a decimal. Adding two different currencies throws.
- The repositories are in memory and start empty. Searching before you post anything
  returns nothing, and that is correct.
- Migrations are forward only. If you edit one that has shipped, test will diverge
  from prod and you will not find out for a fortnight.

TODO: section on the on call rota once the dev container decision lands.
