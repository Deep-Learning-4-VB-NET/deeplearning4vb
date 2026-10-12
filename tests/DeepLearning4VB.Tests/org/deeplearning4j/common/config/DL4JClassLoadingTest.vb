Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.deeplearning4j.common.config
Imports DeepLearning4VB.org.nd4j.common.config
Imports DeepLearning4VB.Tests.org.deeplearning4j.common.config.dummies

Namespace org.deeplearning4j.common.config

    ''' <summary>
    ''' Ported from the DL4J JUnit test "Dl 4 J Class Loading Test". Class names are the .NET full
    ''' names of the dummy types (the Java test used a package prefix).
    ''' </summary>
    <TestClass>
    Public Class DL4JClassLoadingTest

        <TestCleanup>
        Public Sub Cleanup()
            ND4JClassLoading.clearRegisteredServices()
            ND4JClassLoading.resetNd4jClassloader()
        End Sub

        <TestMethod>
        Public Sub testCreateNewInstance_constructorWithoutArguments()
            ' Given
            Dim className As String = GetType(TestDummy).FullName
            ' When
            Dim instance As Object = DL4JClassLoading.createNewInstance(Of Object)(className)
            ' Then
            assertNotNull(instance)
            assertEquals(className, instance.GetType().FullName)
        End Sub

        <TestMethod>
        Public Sub testCreateNewInstance_constructorWithArgument_implicitArgumentTypes()
            Dim className As String = GetType(TestColor).FullName
            Dim instance As TestAbstract = DL4JClassLoading.createNewInstance(Of TestAbstract)(className, GetType(TestAbstract), "white")
            assertNotNull(instance)
            assertEquals(className, instance.GetType().FullName)
            assertEquals("white", DirectCast(instance, TestColor).Name)
        End Sub

        <TestMethod>
        Public Sub testCreateNewInstance_constructorWithArgument_explicitArgumentTypes()
            Dim colorClassName As String = GetType(TestColor).FullName
            Dim rectangleClassName As String = GetType(TestRectangle).FullName
            Dim color As TestAbstract = DL4JClassLoading.createNewInstance(Of TestAbstract)(colorClassName, GetType(Object), New Type() {GetType(Integer), GetType(Integer), GetType(Integer)}, New Object() {45, 175, 200})
            Dim rectangle As TestAbstract = DL4JClassLoading.createNewInstance(Of TestAbstract)(rectangleClassName, GetType(Object), New Type() {GetType(Integer), GetType(Integer), GetType(TestAbstract)}, New Object() {10, 15, color})
            assertNotNull(color)
            assertEquals(colorClassName, color.GetType().FullName)
            assertNotNull(rectangle)
            assertEquals(rectangleClassName, rectangle.GetType().FullName)
            assertEquals(10, DirectCast(rectangle, TestRectangle).Width)
            assertTrue(DirectCast(rectangle, TestRectangle).Color Is color)
        End Sub

        <TestMethod>
        Public Sub testCreateNewInstance_failures()
            assertThrows(Of NullReferenceException)(Function() DL4JClassLoading.createNewInstance(Of Object)("no.such.Type"))
            assertThrows(Of MissingMethodException)(Function() DL4JClassLoading.createNewInstance(Of Object)(GetType(TestDummy).FullName, "unexpected argument"))
            assertThrows(Of InvalidCastException)(Function() DL4JClassLoading.createNewInstance(Of TestAbstract)(GetType(TestDummy).FullName, GetType(TestAbstract)))
        End Sub

        <TestMethod>
        Public Sub testClassLookup()
            assertTrue(DL4JClassLoading.classPresentOnClasspath(GetType(TestDummy).FullName))
            assertTrue(ND4JClassLoading.classPresentOnClasspath("System.Text.StringBuilder"))
            assertFalse(ND4JClassLoading.classPresentOnClasspath("org.deeplearning4j.common.config.dummies.Missing"))
            assertNull(ND4JClassLoading.loadClassByName(Nothing))
            assertTrue(GetType(TestDummy) Is DL4JClassLoading.loadClassByName(GetType(TestDummy).FullName))
            assertTrue(GetType(TestDummy) Is DL4JClassLoading.loadClassByName(GetType(TestDummy).AssemblyQualifiedName))
        End Sub

        <TestMethod>
        Public Sub testLoadService_registeredAndDiscovered()
            Dim registeredImpl As New ServiceImplA()
            ND4JClassLoading.registerService(Of ITestService)(registeredImpl)
            Dim found As IList(Of ITestService) = DL4JClassLoading.loadService(Of ITestService)()
            ' the registered instance comes first, then one discovered instance of each concrete implementation with a parameterless constructor
            assertTrue(found(0) Is registeredImpl)
            Dim names As New List(Of String)()
            For Each s In found
                names.Add(s.GetType().Name)
            Next
            assertTrue(names.Contains("ServiceImplB"), String.Join(",", names))
            assertFalse(names.Contains("ServiceImplNoCtor"), String.Join(",", names))
            assertEquals(1, names.FindAll(Function(n) n = "ServiceImplA").Count, "a registered type is not discovered a second time")
        End Sub

        Public Interface ITestService
        End Interface

        Public Class ServiceImplA
            Implements ITestService
        End Class

        Public Class ServiceImplB
            Implements ITestService
        End Class

        Public Class ServiceImplNoCtor
            Implements ITestService

            Public Sub New(ByVal ignored As Integer)
            End Sub
        End Class

    End Class

End Namespace
