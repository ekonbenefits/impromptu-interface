Imports ImpromptuInterface.Build
Imports NUnit.Framework

Namespace VBNET

    <SetUpFixture()>
    Public Class FixtureSetup

        Private Builder As IDisposable

        <SetUp()>
        Public Sub Setup()

            Builder = BuildProxy.WriteOutDll("ImpromptuEmit.VB")

        End Sub

        <TearDown()>
        Public Sub TearDown()

            Builder.Dispose()
        End Sub
    End Class
End Namespace