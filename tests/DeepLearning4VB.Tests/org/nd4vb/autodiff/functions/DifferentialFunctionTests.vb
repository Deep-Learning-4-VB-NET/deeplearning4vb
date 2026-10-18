Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports DeepLearning4VB.org.nd4vb.autodiff.functions
Imports AttributeAdapter = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException

Namespace org.nd4vb.autodiff.functions

    ''' <summary>
    ''' <see cref="DifferentialFunction"/> outside SameDiff: the property mapping (field values set by reflection with
    ''' Java's type adaptations, the configuration-object path, <see cref="PropertyMapping"/>), the own name, equality
    ''' and the members that need a later milestone.
    ''' </summary>
    <TestClass>
    Public Class DifferentialFunctionTests

        Private Class TestFunction
            Inherits DifferentialFunction

            Public count As Integer
            Public ratio As Single
            Public letter As Char
            Public mode As DataType
            Public name As String
            Public dims As Integer()
            Public boxedDims As Integer?()
            Public longs As Long()
            Public doubles As Double()
            Public floats As Single()

            Public Sub New()
            End Sub

            Public Sub New(ByVal sameDiff As Boolean)
                MyBase.New(sameDiff)
            End Sub

            Public Overrides Function opName() As String
                Return "test_function"
            End Function

            Public Overrides Function onnxName() As String
                Return "TestOnnx"
            End Function

            Public Overrides Function tensorflowName() As String
                Return "TestTf"
            End Function

            Public Overrides Sub clearArrays()
            End Sub
        End Class

        Private Class TestConfig
            Public size As Integer
        End Class

        Private Class ConfigFunction
            Inherits TestFunction

            Private config As TestConfig
            Public fieldName As String

            Public Function getConfig() As TestConfig
                Return config
            End Function

            Public Overrides ReadOnly Property ConfigProperties As Boolean
                Get
                    Return True
                End Get
            End Property

            Public Overrides Function configFieldName() As String
                Return fieldName
            End Function
        End Class

        Private Shared Function Field(ByVal t As Type, ByVal name As String) As FieldInfo
            Return t.GetField(name, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
        End Function

        Private Shared Function F(ByVal name As String) As FieldInfo
            Return Field(GetType(TestFunction), name)
        End Function

        <TestMethod>
        Public Sub MappingsAndAdaptersAreEmptyByDefault()
            Dim fn As New TestFunction()
            assertEquals(0, fn.mappingsForFunction().Count)
            assertEquals(0, fn.attributeAdaptersForFunction().Count)
            assertFalse(fn.ConfigProperties)
            assertNull(fn.configFieldName())
        End Sub

        <TestMethod>
        Public Sub PropertiesForFunctionNeedsModelImport()
            Dim fn As New TestFunction()
            assertThrows(Of NotSupportedException)(Function() fn.propertiesForFunction())
            assertThrows(Of NotSupportedException)(Sub() fn.setPropertiesForFunction(New Dictionary(Of String, Object)()))
        End Sub

        <TestMethod>
        Public Sub SetValueForStoresPlainValues()
            Dim fn As New TestFunction()
            fn.setValueFor(F("count"), 42)
            assertEquals(42, fn.count)
            assertEquals(42, fn.getValue(F("count")))
            fn.setValueFor(F("name"), "abc")
            assertEquals("abc", fn.name)
            fn.setValueFor(F("name"), Nothing)
            assertNull(fn.name)
            fn.setValueFor(F("dims"), New Integer() {1, 2})
            assertArrayEquals(New Integer() {1, 2}, fn.dims)
        End Sub

        <TestMethod>
        Public Sub SetValueForAdaptsTheJavaImportTypes()
            Dim fn As New TestFunction()
            ' floats stored as doubles, chars as integers
            fn.setValueFor(F("ratio"), 0.25)
            assertEquals(0.25F, fn.ratio, 0.0F)
            fn.setValueFor(F("letter"), 65)
            assertEquals("A"c, fn.letter)
            ' enums from their names, ignoring case
            fn.setValueFor(F("mode"), "float")
            assertEquals(DataType.FLOAT, fn.mode)
            assertThrows(Of InvalidOperationException)(Sub() fn.setValueFor(F("mode"), "no_such_type"))
            ' a single number for an array field becomes a one-element array, converted like Java's Number
            fn.setValueFor(F("dims"), 3.7)
            assertArrayEquals(New Integer() {3}, fn.dims)
            fn.setValueFor(F("boxedDims"), 4L)
            assertEquals(1, fn.boxedDims.Length)
            assertEquals(4, fn.boxedDims(0).Value)
            fn.setValueFor(F("longs"), 5)
            assertArrayEquals(New Long() {5L}, fn.longs)
            fn.setValueFor(F("doubles"), 2)
            assertArrayEquals(New Double() {2.0}, fn.doubles, 0.0)
            fn.setValueFor(F("floats"), 1.5)
            assertArrayEquals(New Single() {1.5F}, fn.floats, 0.0F)
        End Sub

        <TestMethod>
        Public Sub SetValueForRejectsNothingForPrimitives()
            Dim fn As New TestFunction()
            assertThrows(Of ND4VBIllegalStateException)(Sub() fn.setValueFor(F("count"), Nothing))
        End Sub

        <TestMethod>
        Public Sub ConfigPropertiesGoToTheConfigurationObject()
            Dim fn As New ConfigFunction()
            assertNull(fn.getConfig())
            fn.setValueFor(Field(GetType(TestConfig), "size"), 7)
            assertNotNull(fn.getConfig())
            assertEquals(7, fn.getConfig().size)
            fn.setValueFor(Field(GetType(TestConfig), "size"), 8)
            assertEquals(8, fn.getConfig().size)

            Dim missing As New ConfigFunction() With {.fieldName = "noSuchField"}
            assertThrows(Of InvalidOperationException)(Sub() missing.setValueFor(Field(GetType(TestConfig), "size"), 1))
        End Sub

        <TestMethod>
        Public Sub AttributeAdaptersSetFieldsThroughTheFunction()
            Dim fn As New TestFunction()
            Dim adapter As AttributeAdapter = New PlusOneAdapter()
            adapter.mapAttributeFor(41, F("count"), fn)
            assertEquals(42, fn.count)
        End Sub

        Private NotInheritable Class PlusOneAdapter
            Implements AttributeAdapter

            Public Sub mapAttributeFor(ByVal inputAttributeValue As Object, ByVal fieldFor As FieldInfo, ByVal [on] As DifferentialFunction) Implements AttributeAdapter.mapAttributeFor
                [on].setValueFor(fieldFor, CInt(inputAttributeValue) + 1)
            End Sub
        End Class

        <TestMethod>
        Public Sub PropertyMappingBuilderAndValueSemantics()
            Dim a As PropertyMapping = PropertyMapping.builder().propertyNames("x", "y").tfInputPosition(1).tfAttrName("T").build()
            assertArrayEquals(New String() {"x", "y"}, a.getPropertyNames())
            assertArrayEquals(New String() {"x", "y"}, a.PropertyNames)
            assertEquals(1, a.getTfInputPosition().Value)
            assertEquals("T", a.TfAttrName)
            assertNull(a.getOnnxAttrName())
            assertFalse(a.ShapePosition.HasValue)
            assertEquals("PropertyMapping(propertyNames=[x, y], tfInputPosition=1, onnxAttrName=null, tfAttrName=T, shapePosition=null)", a.ToString())

            Dim b As PropertyMapping = PropertyMapping.builder().propertyNames("x", "y").tfInputPosition(1).tfAttrName("T").build()
            assertTrue(a.Equals(b))
            assertEquals(a.GetHashCode(), b.GetHashCode())
            b.setOnnxAttrName("axis")
            assertFalse(a.Equals(b))
            b.setOnnxAttrName(Nothing)
            b.ShapePosition = 0
            assertFalse(a.Equals(b))
        End Sub

        <TestMethod>
        Public Sub TheOwnNameIsARandomGuidOutsideSameDiff()
            Dim fn As New TestFunction()
            assertTrue(Guid.TryParse(fn.getOwnName(), Nothing))
            assertTrue(fn.isOwnNameSetWithDefault())
            Dim noId As New TestFunction(False)
            assertNull(noId.getOwnName())
            assertFalse(noId.isOwnNameSetWithDefault())
        End Sub

        <TestMethod>
        Public Sub EqualityComparesInPlaceDimensionsScalarAndOwnName()
            Dim a As New TestFunction()
            Dim b As New TestFunction()
            assertFalse(a.Equals(b))
            b.setOwnName(a.getOwnName())
            assertTrue(a.Equals(b))
            assertEquals(a.GetHashCode(), b.GetHashCode())
            a.setDimensions(1, 2)
            assertFalse(a.Equals(b))
            b.setDimensions(1, 2)
            assertTrue(a.Equals(b))
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertArrayEquals(New Integer() {1, 2}, a.getDimensions())
            b.setInPlace(True)
            assertTrue(b.isInPlace())
            assertFalse(a.Equals(b))
            assertFalse(a.Equals(New ConfigFunction()))
        End Sub

        <TestMethod>
        Public Sub DefaultsOfTheOpMembers()
            Dim fn As New TestFunction()
            assertArrayEquals(New String() {"TestOnnx"}, fn.onnxNames())
            assertArrayEquals(New String() {"TestTf"}, fn.tensorflowNames())
            assertEquals(-1, fn.NumOutputs)
            assertNull(fn.getExtraArgs())
            fn.setExtraArgs(New Object() {1.0})
            assertArrayEquals(New Object() {1.0}, fn.getExtraArgs())
            assertNull(fn.getScalarValue())
            assertThrows(Of NotSupportedException)(Function() fn.opNum())
            assertThrows(Of NotSupportedException)(Function() fn.opType())
            assertThrows(Of NotSupportedException)(Function() fn.getInputArgument(0))
            assertThrows(Of NotSupportedException)(Function() fn.dup())
            assertThrows(Of ND4VBIllegalStateException)(Function() fn.calculateOutputShape())
            assertThrows(Of NotSupportedException)(Function() fn.calculateOutputDataTypes(New List(Of DataType)()))
        End Sub

    End Class

End Namespace
