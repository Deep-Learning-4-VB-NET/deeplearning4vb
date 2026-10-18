Imports System
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.enums

Namespace org.nd4vb.linalg.api.memory.conf

    ''' <summary>
    ''' The configuration of a workspace: allocation, spill, mirroring, learning, reset and location policies and the
    ''' sizes. Lombok's <c>@Builder @Data @NoArgsConstructor @AllArgsConstructor</c> members are written out, with the
    ''' <c>@Builder.Default</c> values as the defaults of both the no-argument constructor and the builder:
    ''' <c>WorkspaceConfiguration.builder().initialSize(10 * 1024 * 1024).policyLearning(LearningPolicy.NONE).build()</c>.
    ''' In the managed backend memory always comes from the garbage-collected heap, so the policies only affect the
    ''' bookkeeping (see docs/PHASE2_DESIGN.md).
    ''' </summary>
    <Serializable>
    Public Class WorkspaceConfiguration

        Protected Friend policyAllocation As AllocationPolicy = AllocationPolicy.OVERALLOCATE
        Protected Friend policySpill As SpillPolicy = SpillPolicy.EXTERNAL
        Protected Friend policyMirroring As MirroringPolicy = MirroringPolicy.FULL
        Protected Friend policyLearning As LearningPolicy = LearningPolicy.FIRST_LOOP
        Protected Friend policyReset As ResetPolicy = ResetPolicy.BLOCK_LEFT
        Protected Friend policyLocation As LocationPolicy = LocationPolicy.RAM
        ''' <summary>Path to file to be memory-mapped</summary>
        Protected Friend tempFilePath As String = Nothing
        ''' <summary>This variable specifies amount of memory allocated for this workspace during initialization</summary>
        Protected Friend initialSize As Long = 0
        ''' <summary>This variable specifies minimal workspace size</summary>
        Protected Friend minSize As Long = 0
        ''' <summary>This variable specifies maximal workspace size</summary>
        Protected Friend maxSize As Long = 0
        ''' <summary>For workspaces with learnable size, this variable defines how many cycles will be spent during learning phase</summary>
        Protected Friend cyclesBeforeInitialization As Integer = 0
        ''' <summary>If OVERALLOCATION policy is set, memory will be overallocated in addition to initialSize of learned size</summary>
        Protected Friend overallocationLimit As Double = 0.3
        ''' <summary>This value is used only for circular workspaces</summary>
        Protected Friend stepsNumber As Integer = 2

        Public Sub New()
        End Sub

        Public Sub New(ByVal policyAllocation As AllocationPolicy, ByVal policySpill As SpillPolicy, ByVal policyMirroring As MirroringPolicy, ByVal policyLearning As LearningPolicy, ByVal policyReset As ResetPolicy, ByVal policyLocation As LocationPolicy, ByVal tempFilePath As String, ByVal initialSize As Long, ByVal minSize As Long, ByVal maxSize As Long, ByVal cyclesBeforeInitialization As Integer, ByVal overallocationLimit As Double, ByVal stepsNumber As Integer)
            Me.policyAllocation = policyAllocation
            Me.policySpill = policySpill
            Me.policyMirroring = policyMirroring
            Me.policyLearning = policyLearning
            Me.policyReset = policyReset
            Me.policyLocation = policyLocation
            Me.tempFilePath = tempFilePath
            Me.initialSize = initialSize
            Me.minSize = minSize
            Me.maxSize = maxSize
            Me.cyclesBeforeInitialization = cyclesBeforeInitialization
            Me.overallocationLimit = overallocationLimit
            Me.stepsNumber = stepsNumber
        End Sub

        Public Shared Function builder() As WorkspaceConfigurationBuilder
            Return New WorkspaceConfigurationBuilder()
        End Function

        Public Overridable Function getPolicyAllocation() As AllocationPolicy
            Return policyAllocation
        End Function

        Public Overridable Sub setPolicyAllocation(ByVal policyAllocation As AllocationPolicy)
            Me.policyAllocation = policyAllocation
        End Sub

        Public Overridable Function getPolicySpill() As SpillPolicy
            Return policySpill
        End Function

        Public Overridable Sub setPolicySpill(ByVal policySpill As SpillPolicy)
            Me.policySpill = policySpill
        End Sub

        Public Overridable Function getPolicyMirroring() As MirroringPolicy
            Return policyMirroring
        End Function

        Public Overridable Sub setPolicyMirroring(ByVal policyMirroring As MirroringPolicy)
            Me.policyMirroring = policyMirroring
        End Sub

        Public Overridable Function getPolicyLearning() As LearningPolicy
            Return policyLearning
        End Function

        Public Overridable Sub setPolicyLearning(ByVal policyLearning As LearningPolicy)
            Me.policyLearning = policyLearning
        End Sub

        Public Overridable Function getPolicyReset() As ResetPolicy
            Return policyReset
        End Function

        Public Overridable Sub setPolicyReset(ByVal policyReset As ResetPolicy)
            Me.policyReset = policyReset
        End Sub

        Public Overridable Function getPolicyLocation() As LocationPolicy
            Return policyLocation
        End Function

        Public Overridable Sub setPolicyLocation(ByVal policyLocation As LocationPolicy)
            Me.policyLocation = policyLocation
        End Sub

        Public Overridable Function getTempFilePath() As String
            Return tempFilePath
        End Function

        Public Overridable Sub setTempFilePath(ByVal tempFilePath As String)
            Me.tempFilePath = tempFilePath
        End Sub

        Public Overridable Function getInitialSize() As Long
            Return initialSize
        End Function

        Public Overridable Sub setInitialSize(ByVal initialSize As Long)
            Me.initialSize = initialSize
        End Sub

        Public Overridable Function getMinSize() As Long
            Return minSize
        End Function

        Public Overridable Sub setMinSize(ByVal minSize As Long)
            Me.minSize = minSize
        End Sub

        Public Overridable Function getMaxSize() As Long
            Return maxSize
        End Function

        Public Overridable Sub setMaxSize(ByVal maxSize As Long)
            Me.maxSize = maxSize
        End Sub

        Public Overridable Function getCyclesBeforeInitialization() As Integer
            Return cyclesBeforeInitialization
        End Function

        Public Overridable Sub setCyclesBeforeInitialization(ByVal cyclesBeforeInitialization As Integer)
            Me.cyclesBeforeInitialization = cyclesBeforeInitialization
        End Sub

        Public Overridable Function getOverallocationLimit() As Double
            Return overallocationLimit
        End Function

        Public Overridable Sub setOverallocationLimit(ByVal overallocationLimit As Double)
            Me.overallocationLimit = overallocationLimit
        End Sub

        Public Overridable Function getStepsNumber() As Integer
            Return stepsNumber
        End Function

        Public Overridable Sub setStepsNumber(ByVal stepsNumber As Integer)
            Me.stepsNumber = stepsNumber
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As WorkspaceConfiguration = TryCast(obj, WorkspaceConfiguration)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return policyAllocation.Equals(o.policyAllocation) AndAlso policySpill.Equals(o.policySpill) AndAlso policyMirroring.Equals(o.policyMirroring) AndAlso policyLearning.Equals(o.policyLearning) AndAlso policyReset.Equals(o.policyReset) AndAlso policyLocation.Equals(o.policyLocation) AndAlso Object.Equals(tempFilePath, o.tempFilePath) AndAlso initialSize.Equals(o.initialSize) AndAlso minSize.Equals(o.minSize) AndAlso maxSize.Equals(o.maxSize) AndAlso cyclesBeforeInitialization.Equals(o.cyclesBeforeInitialization) AndAlso overallocationLimit.Equals(o.overallocationLimit) AndAlso stepsNumber.Equals(o.stepsNumber)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + policyAllocation.GetHashCode()
            h = h * 59 + policySpill.GetHashCode()
            h = h * 59 + policyMirroring.GetHashCode()
            h = h * 59 + policyLearning.GetHashCode()
            h = h * 59 + policyReset.GetHashCode()
            h = h * 59 + policyLocation.GetHashCode()
            h = h * 59 + If(tempFilePath Is Nothing, 43, tempFilePath.GetHashCode())
            h = h * 59 + initialSize.GetHashCode()
            h = h * 59 + minSize.GetHashCode()
            h = h * 59 + maxSize.GetHashCode()
            h = h * 59 + cyclesBeforeInitialization.GetHashCode()
            h = h * 59 + overallocationLimit.GetHashCode()
            h = h * 59 + stepsNumber.GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "WorkspaceConfiguration(" & "policyAllocation=" & policyAllocation.ToString() & ", policySpill=" & policySpill.ToString() & ", policyMirroring=" & policyMirroring.ToString() & ", policyLearning=" & policyLearning.ToString() & ", policyReset=" & policyReset.ToString() & ", policyLocation=" & policyLocation.ToString() & ", tempFilePath=" & Objects.toString(tempFilePath) & ", initialSize=" & initialSize.ToString() & ", minSize=" & minSize.ToString() & ", maxSize=" & maxSize.ToString() & ", cyclesBeforeInitialization=" & cyclesBeforeInitialization.ToString() & ", overallocationLimit=" & Arrays.JavaDoubleToString(overallocationLimit) & ", stepsNumber=" & stepsNumber.ToString() & ")"
        End Function

        ''' <summary>Lombok's builder for <see cref="WorkspaceConfiguration"/>.</summary>
        Public Class WorkspaceConfigurationBuilder

            Private policyAllocationValue As AllocationPolicy = AllocationPolicy.OVERALLOCATE
            Private policySpillValue As SpillPolicy = SpillPolicy.EXTERNAL
            Private policyMirroringValue As MirroringPolicy = MirroringPolicy.FULL
            Private policyLearningValue As LearningPolicy = LearningPolicy.FIRST_LOOP
            Private policyResetValue As ResetPolicy = ResetPolicy.BLOCK_LEFT
            Private policyLocationValue As LocationPolicy = LocationPolicy.RAM
            Private tempFilePathValue As String = Nothing
            Private initialSizeValue As Long = 0
            Private minSizeValue As Long = 0
            Private maxSizeValue As Long = 0
            Private cyclesBeforeInitializationValue As Integer = 0
            Private overallocationLimitValue As Double = 0.3
            Private stepsNumberValue As Integer = 2

            Public Function policyAllocation(ByVal value As AllocationPolicy) As WorkspaceConfigurationBuilder
                Me.policyAllocationValue = value
                Return Me
            End Function

            Public Function policySpill(ByVal value As SpillPolicy) As WorkspaceConfigurationBuilder
                Me.policySpillValue = value
                Return Me
            End Function

            Public Function policyMirroring(ByVal value As MirroringPolicy) As WorkspaceConfigurationBuilder
                Me.policyMirroringValue = value
                Return Me
            End Function

            Public Function policyLearning(ByVal value As LearningPolicy) As WorkspaceConfigurationBuilder
                Me.policyLearningValue = value
                Return Me
            End Function

            Public Function policyReset(ByVal value As ResetPolicy) As WorkspaceConfigurationBuilder
                Me.policyResetValue = value
                Return Me
            End Function

            Public Function policyLocation(ByVal value As LocationPolicy) As WorkspaceConfigurationBuilder
                Me.policyLocationValue = value
                Return Me
            End Function

            Public Function tempFilePath(ByVal value As String) As WorkspaceConfigurationBuilder
                Me.tempFilePathValue = value
                Return Me
            End Function

            Public Function initialSize(ByVal value As Long) As WorkspaceConfigurationBuilder
                Me.initialSizeValue = value
                Return Me
            End Function

            Public Function minSize(ByVal value As Long) As WorkspaceConfigurationBuilder
                Me.minSizeValue = value
                Return Me
            End Function

            Public Function maxSize(ByVal value As Long) As WorkspaceConfigurationBuilder
                Me.maxSizeValue = value
                Return Me
            End Function

            Public Function cyclesBeforeInitialization(ByVal value As Integer) As WorkspaceConfigurationBuilder
                Me.cyclesBeforeInitializationValue = value
                Return Me
            End Function

            Public Function overallocationLimit(ByVal value As Double) As WorkspaceConfigurationBuilder
                Me.overallocationLimitValue = value
                Return Me
            End Function

            Public Function stepsNumber(ByVal value As Integer) As WorkspaceConfigurationBuilder
                Me.stepsNumberValue = value
                Return Me
            End Function

            Public Function build() As WorkspaceConfiguration
                Return New WorkspaceConfiguration(policyAllocationValue, policySpillValue, policyMirroringValue, policyLearningValue, policyResetValue, policyLocationValue, tempFilePathValue, initialSizeValue, minSizeValue, maxSizeValue, cyclesBeforeInitializationValue, overallocationLimitValue, stepsNumberValue)
            End Function

        End Class

    End Class

End Namespace
