Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer.util

Namespace org.nd4vb.linalg.api.buffer.util

    ''' <summary>
    ''' Tests for the conversions of <see cref="DataTypeUtil"/> and <see cref="AllocUtil"/> (the members that read or write the
    ''' backend properties need <c>Nd4vb.Backend</c>, which arrives in wave 3).
    ''' </summary>
    <TestClass>
    Public Class BufferUtilTests

        <TestMethod>
        Public Sub testDataTypeUtil()
            assertEquals(8, DataTypeUtil.lengthForDtype(DataType.DOUBLE))
            assertEquals(4, DataTypeUtil.lengthForDtype(DataType.FLOAT))
            assertEquals(4, DataTypeUtil.lengthForDtype(DataType.INT))
            assertEquals(2, DataTypeUtil.lengthForDtype(DataType.HALF))
            assertEquals(8, DataTypeUtil.lengthForDtype(DataType.LONG))
            assertThrows(Of ArgumentException)(Sub() DataTypeUtil.lengthForDtype(DataType.BOOL))

            assertEquals(DataType.DOUBLE, DataTypeUtil.getDtypeFromContext("double"))
            assertEquals(DataType.FLOAT, DataTypeUtil.getDtypeFromContext("float"))
            assertEquals(DataType.INT, DataTypeUtil.getDtypeFromContext("int"))
            assertEquals(DataType.HALF, DataTypeUtil.getDtypeFromContext("half"))
            assertEquals(DataType.FLOAT, DataTypeUtil.getDtypeFromContext("other"))
            assertEquals(DataType.FLOAT, DataTypeUtil.getDtypeFromContext(DirectCast(Nothing, String)))

            assertEquals("double", DataTypeUtil.getDTypeForName(DataType.DOUBLE))
            assertEquals("half", DataTypeUtil.getDTypeForName(DataType.HALF))
            assertEquals("int", DataTypeUtil.getDTypeForName(DataType.INT))
            assertEquals("float", DataTypeUtil.getDTypeForName(DataType.LONG))
            assertThrows(Of ArgumentException)(Sub() DataTypeUtil.setDTypeForContext("long"))
        End Sub

        <TestMethod>
        Public Sub testAllocUtil()
            assertEquals(DataBuffer.AllocationMode.HEAP, AllocUtil.getAllocationModeFromContext("heap"))
            assertEquals(DataBuffer.AllocationMode.DIRECT, AllocUtil.getAllocationModeFromContext("direct"))
            assertEquals(DataBuffer.AllocationMode.JAVACPP, AllocUtil.getAllocationModeFromContext("javacpp"))
            assertEquals(DataBuffer.AllocationMode.JAVACPP, AllocUtil.getAllocationModeFromContext("x"))
            assertEquals("heap", AllocUtil.getAllocModeName(DataBuffer.AllocationMode.HEAP))
            assertEquals("direct", AllocUtil.getAllocModeName(DataBuffer.AllocationMode.DIRECT))
            assertEquals("javacpp", AllocUtil.getAllocModeName(DataBuffer.AllocationMode.MIXED_DATA_TYPES))
            assertEquals(DataBuffer.AllocationMode.MIXED_DATA_TYPES, AllocUtil.AllocationModeFromContext)
            assertEquals(DataBuffer.AllocationMode.MIXED_DATA_TYPES, AllocUtil.getAllocationModeFromContext())
            assertThrows(Of ArgumentException)(Sub() AllocUtil.setAllocationModeForContext("mixed"))
        End Sub

    End Class

End Namespace
