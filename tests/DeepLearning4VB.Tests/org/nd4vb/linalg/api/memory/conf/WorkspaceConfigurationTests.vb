Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.util
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.conf
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.enums

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.api.memory.conf

    ''' <summary>Tests for <c>WorkspaceConfiguration</c>: Lombok's builder, the defaults, equality and serialization.</summary>
    <TestClass>
    Public Class WorkspaceConfigurationTests

        Private Shared Sub assertJavaDefaults(ByVal c As WorkspaceConfiguration)
            assertEquals(AllocationPolicy.OVERALLOCATE, c.getPolicyAllocation())
            assertEquals(SpillPolicy.EXTERNAL, c.getPolicySpill())
            assertEquals(MirroringPolicy.FULL, c.getPolicyMirroring())
            assertEquals(LearningPolicy.FIRST_LOOP, c.getPolicyLearning())
            assertEquals(ResetPolicy.BLOCK_LEFT, c.getPolicyReset())
            assertEquals(LocationPolicy.RAM, c.getPolicyLocation())
            assertNull(c.getTempFilePath())
            assertEquals(0L, c.getInitialSize())
            assertEquals(0L, c.getMinSize())
            assertEquals(0L, c.getMaxSize())
            assertEquals(0, c.getCyclesBeforeInitialization())
            assertEquals(0.3, c.getOverallocationLimit(), 0.0)
            assertEquals(2, c.getStepsNumber())
        End Sub

        <TestMethod>
        Public Sub testBuilderDefaultsAreJavas()
            assertJavaDefaults(WorkspaceConfiguration.builder().build())
            ' Lombok's @NoArgsConstructor keeps the @Builder.Default values too
            assertJavaDefaults(New WorkspaceConfiguration())
            assertEquals(WorkspaceConfiguration.builder().build(), New WorkspaceConfiguration())
        End Sub

        <TestMethod>
        Public Sub testBuilderSetsEveryField()
            Dim c As WorkspaceConfiguration = WorkspaceConfiguration.builder() _
                .policyAllocation(AllocationPolicy.STRICT) _
                .policySpill(SpillPolicy.REALLOCATE) _
                .policyMirroring(MirroringPolicy.HOST_ONLY) _
                .policyLearning(LearningPolicy.OVER_TIME) _
                .policyReset(ResetPolicy.ENDOFBUFFER_REACHED) _
                .policyLocation(LocationPolicy.MMAP) _
                .tempFilePath("ws.bin") _
                .initialSize(10 * 1024 * 1024) _
                .minSize(1024) _
                .maxSize(1L << 32) _
                .cyclesBeforeInitialization(3) _
                .overallocationLimit(0.5) _
                .stepsNumber(4) _
                .build()
            assertEquals(AllocationPolicy.STRICT, c.getPolicyAllocation())
            assertEquals(SpillPolicy.REALLOCATE, c.getPolicySpill())
            assertEquals(MirroringPolicy.HOST_ONLY, c.getPolicyMirroring())
            assertEquals(LearningPolicy.OVER_TIME, c.getPolicyLearning())
            assertEquals(ResetPolicy.ENDOFBUFFER_REACHED, c.getPolicyReset())
            assertEquals(LocationPolicy.MMAP, c.getPolicyLocation())
            assertEquals("ws.bin", c.getTempFilePath())
            assertEquals(10L * 1024 * 1024, c.getInitialSize())
            assertEquals(1024L, c.getMinSize())
            assertEquals(1L << 32, c.getMaxSize())
            assertEquals(3, c.getCyclesBeforeInitialization())
            assertEquals(0.5, c.getOverallocationLimit(), 0.0)
            assertEquals(4, c.getStepsNumber())

            ' the builder leaves unset fields at their defaults
            Dim partialConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(100).policyLearning(LearningPolicy.NONE).build()
            assertEquals(100L, partialConfig.getInitialSize())
            assertEquals(LearningPolicy.NONE, partialConfig.getPolicyLearning())
            assertEquals(AllocationPolicy.OVERALLOCATE, partialConfig.getPolicyAllocation())
            assertEquals(2, partialConfig.getStepsNumber())
        End Sub

        <TestMethod>
        Public Sub testSettersEqualityAndToString()
            Dim a As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(5).build()
            Dim b As New WorkspaceConfiguration()
            assertNotEquals(a, b)
            b.setInitialSize(5)
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            b.setPolicySpill(SpillPolicy.FAIL)
            assertNotEquals(a, b)
            b.setPolicySpill(SpillPolicy.EXTERNAL)
            b.setTempFilePath("x")
            assertNotEquals(a, b)

            assertEquals("WorkspaceConfiguration(policyAllocation=OVERALLOCATE, policySpill=EXTERNAL, policyMirroring=FULL, " &
                         "policyLearning=FIRST_LOOP, policyReset=BLOCK_LEFT, policyLocation=RAM, tempFilePath=null, initialSize=5, " &
                         "minSize=0, maxSize=0, cyclesBeforeInitialization=0, overallocationLimit=0.3, stepsNumber=2)", a.ToString())
        End Sub

        <TestMethod>
        Public Sub testSerializationRoundTrip()
            Dim c As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(64).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).tempFilePath("t").build()
            Dim back As WorkspaceConfiguration = SerializationUtils.deserialize(Of WorkspaceConfiguration)(SerializationUtils.toByteArray(c))
            assertNotNull(back)
            assertEquals(c, back)
            Assert.AreNotSame(c, back)
        End Sub

    End Class

End Namespace
