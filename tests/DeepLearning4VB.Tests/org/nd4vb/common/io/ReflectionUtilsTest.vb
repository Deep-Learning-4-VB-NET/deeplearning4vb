Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.io

Namespace org.nd4vb.common.io

    Public Class ReflectionBase
        Public Shared ReadOnly CONSTANT As String = "c"
        Public Const LITERAL As Integer = 3
        Private baseValue As Integer = 1
        Protected ReadOnly frozen As String = "base"

        Public Function getBaseValue() As Integer
            Return baseValue
        End Function

        Public Overridable Function describe() As String
            Return "base"
        End Function

        Public Overrides Function ToString() As String
            Return "ReflectionBase:" & frozen
        End Function
    End Class

    Public Class ReflectionChild
        Inherits ReflectionBase

        Public name As String = "child"
        Private secret As Integer = 42

        Public Overrides Function describe() As String
            Return "child"
        End Function

        Public Function add(ByVal a As Integer, ByVal b As Integer) As Integer
            Return a + b
        End Function

        Private Function hidden() As String
            Return "hidden"
        End Function

        Public Sub explode()
            Throw New InvalidOperationException("boom")
        End Sub

        Public Function getSecret() As Integer
            Return secret
        End Function
    End Class

    Public Class Constructed
        Public ReadOnly text As String
        Public ReadOnly number As Integer

        Public Sub New(ByVal text As String, ByVal number As Integer)
            Me.text = text
            Me.number = number
        End Sub

        Friend Sub New(ByVal text As String)
            Me.text = text
        End Sub
    End Class

    <TestClass>
    Public Class ReflectionUtilsTest

        <TestMethod>
        Public Sub FindsFieldsAcrossTheHierarchy()
            Dim nameField = ReflectionUtils.findField(GetType(ReflectionChild), "name")
            assertNotNull(nameField)
            assertEquals(GetType(ReflectionChild), nameField.DeclaringType)
            Dim inherited = ReflectionUtils.findField(GetType(ReflectionChild), "baseValue")
            assertNotNull(inherited)
            assertEquals(GetType(ReflectionBase), inherited.DeclaringType)
            assertNull(ReflectionUtils.findField(GetType(ReflectionChild), "missing"))
            assertNull(ReflectionUtils.findField(GetType(ReflectionChild), "name", GetType(Integer)))
            assertEquals("secret", ReflectionUtils.findField(GetType(ReflectionChild), Nothing, GetType(Integer)).Name)
            assertThrows(Of ArgumentException)(Function() ReflectionUtils.findField(GetType(ReflectionChild), Nothing, Nothing))
            assertThrows(Of ArgumentException)(Function() ReflectionUtils.findField(Nothing, "x"))
        End Sub

        <TestMethod>
        Public Sub GetsAndSetsFields()
            Dim child As New ReflectionChild()
            Dim secret = ReflectionUtils.findField(GetType(ReflectionChild), "secret")
            ReflectionUtils.makeAccessible(secret)
            assertEquals(42, ReflectionUtils.getField(secret, child))
            ReflectionUtils.setField(secret, child, 7)
            assertEquals(7, child.getSecret())
            Dim constant = ReflectionUtils.findField(GetType(ReflectionBase), "CONSTANT")
            assertTrue(ReflectionUtils.isPublicStaticFinal(constant))
            assertTrue(ReflectionUtils.isPublicStaticFinal(ReflectionUtils.findField(GetType(ReflectionBase), "LITERAL")))
            assertFalse(ReflectionUtils.isPublicStaticFinal(ReflectionUtils.findField(GetType(ReflectionChild), "name")))
            assertTrue(ReflectionUtils.COPYABLE_FIELDS.matches(ReflectionUtils.findField(GetType(ReflectionChild), "name")))
            assertFalse(ReflectionUtils.COPYABLE_FIELDS.matches(constant))
            assertFalse(ReflectionUtils.COPYABLE_FIELDS.matches(ReflectionUtils.findField(GetType(ReflectionBase), "frozen")))
        End Sub

        <TestMethod>
        Public Sub FindsAndInvokesMethods()
            Dim add = ReflectionUtils.findMethod(GetType(ReflectionChild), "add", GetType(Integer), GetType(Integer))
            assertNotNull(add)
            assertEquals(5, ReflectionUtils.invokeMethod(add, New ReflectionChild(), 2, 3))
            assertNull(ReflectionUtils.findMethod(GetType(ReflectionChild), "add", GetType(String)))
            assertNull(ReflectionUtils.findMethod(GetType(ReflectionChild), "add"))
            assertNotNull(ReflectionUtils.findMethod(GetType(ReflectionChild), "add", DirectCast(Nothing, Type())))
            Dim inherited = ReflectionUtils.findMethod(GetType(ReflectionChild), "getBaseValue")
            assertEquals(GetType(ReflectionBase), inherited.DeclaringType)
            assertEquals(1, ReflectionUtils.invokeMethod(inherited, New ReflectionChild()))
            Dim hidden = ReflectionUtils.findMethod(GetType(ReflectionChild), "hidden")
            ReflectionUtils.makeAccessible(hidden)
            assertEquals("hidden", ReflectionUtils.invokeMethod(hidden, New ReflectionChild()))
            assertEquals(GetType(ReflectionChild), ReflectionUtils.findMethod(GetType(ReflectionChild), "describe").DeclaringType)
            Dim fromInterface = ReflectionUtils.findMethod(GetType(IList(Of String)), "Add", GetType(String))
            assertNotNull(fromInterface)
            assertEquals(GetType(ICollection(Of String)), fromInterface.DeclaringType)
            Dim explode = ReflectionUtils.findMethod(GetType(ReflectionChild), "explode")
            Dim ex = assertThrows(Of InvalidOperationException)(Function() ReflectionUtils.invokeMethod(explode, New ReflectionChild()))
            assertEquals("boom", ex.Message)
        End Sub

        <TestMethod>
        Public Sub RecognisesObjectMethods()
            Dim equalsMethod = GetType(Object).GetMethod("Equals", New Type() {GetType(Object)})
            assertTrue(ReflectionUtils.isEqualsMethod(equalsMethod))
            assertTrue(ReflectionUtils.isObjectMethod(equalsMethod))
            assertTrue(ReflectionUtils.isHashCodeMethod(GetType(Object).GetMethod("GetHashCode")))
            assertTrue(ReflectionUtils.isToStringMethod(GetType(ReflectionBase).GetMethod("ToString")))
            assertTrue(ReflectionUtils.isObjectMethod(GetType(ReflectionBase).GetMethod("ToString")))
            Dim describe = GetType(ReflectionChild).GetMethod("describe")
            assertFalse(ReflectionUtils.isEqualsMethod(describe))
            assertFalse(ReflectionUtils.isObjectMethod(describe))
            assertFalse(ReflectionUtils.isEqualsMethod(Nothing))
            assertTrue(ReflectionUtils.USER_DECLARED_METHODS.matches(describe))
            assertFalse(ReflectionUtils.USER_DECLARED_METHODS.matches(equalsMethod))
            assertTrue(ReflectionUtils.NON_BRIDGED_METHODS.matches(describe))
            assertFalse(ReflectionUtils.declaresException(describe, GetType(Exception)))
            assertFalse(ReflectionUtils.isCglibRenamedMethod(describe))
        End Sub

        <TestMethod>
        Public Sub WalksMethodsAndFields()
            Dim names As New List(Of String)()
            ReflectionUtils.doWithMethods(GetType(ReflectionChild), Sub(m) names.Add(m.Name), Function(m) m.DeclaringType IsNot GetType(Object))
            assertTrue(names.Contains("add"))
            assertTrue(names.Contains("getBaseValue"))
            assertTrue(names.Contains("hidden"))
            assertFalse(names.Contains("GetHashCode"))
            assertEquals(2, names.Where(Function(n) n = "describe").Count())
            Dim unique = ReflectionUtils.getUniqueDeclaredMethods(GetType(ReflectionChild))
            assertEquals(1, unique.Where(Function(m) m.Name = "describe").Count())
            assertEquals(GetType(ReflectionChild), unique.First(Function(m) m.Name = "describe").DeclaringType)
            Dim everything = ReflectionUtils.getAllDeclaredMethods(GetType(ReflectionChild))
            assertEquals(2, everything.Where(Function(m) m.Name = "describe").Count())
            assertTrue(everything.Any(Function(m) m.Name = "GetHashCode"))
            Dim fieldNames As New List(Of String)()
            ReflectionUtils.doWithFields(GetType(ReflectionChild), Sub(f) fieldNames.Add(f.Name))
            assertTrue(fieldNames.Contains("name"))
            assertTrue(fieldNames.Contains("secret"))
            assertTrue(fieldNames.Contains("baseValue"))
            assertTrue(fieldNames.Contains("CONSTANT"))
            Dim copyable As New List(Of String)()
            ReflectionUtils.doWithFields(GetType(ReflectionChild), Sub(f) copyable.Add(f.Name), ReflectionUtils.COPYABLE_FIELDS)
            assertTrue(copyable.Contains("name"))
            assertFalse(copyable.Contains("CONSTANT"))
            assertFalse(copyable.Contains("frozen"))
        End Sub

        <TestMethod>
        Public Sub CopiesFieldStateAndCreatesInstances()
            Dim source As New ReflectionChild()
            source.name = "copied"
            ReflectionUtils.setField(ReflectionUtils.findField(GetType(ReflectionChild), "secret"), source, 99)
            Dim target As New ReflectionChild()
            ReflectionUtils.shallowCopyFieldState(source, target)
            assertEquals("copied", target.name)
            assertEquals(99, target.getSecret())
            assertThrows(Of ArgumentException)(Sub() ReflectionUtils.shallowCopyFieldState(source, New ReflectionBase()))
            assertThrows(Of ArgumentException)(Sub() ReflectionUtils.shallowCopyFieldState(Nothing, target))

            Dim built = ReflectionUtils.newInstance(Of Constructed)(GetType(Constructed), "t", 5)
            assertEquals("t", built.text)
            assertEquals(5, built.number)
            assertThrows(Of ArgumentException)(Function() ReflectionUtils.newInstance(Of Constructed)(GetType(Constructed), "friend only"))
            assertThrows(Of InvalidOperationException)(Function() ReflectionUtils.newInstance(Of Constructed)(GetType(Constructed), 1.5))
            assertThrows(Of ArgumentException)(Function() ReflectionUtils.newInstance(Of Constructed)(GetType(Constructed), "x", Nothing))
        End Sub

    End Class

End Namespace
